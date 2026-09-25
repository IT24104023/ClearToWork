using ClearToWork.Domain.Entities.Workforce;
using ClearToWork.Domain.Enums;

namespace ClearToWork.Infrastructure.Helpers;

/// <summary>
/// Urgency level classifications for certification expirations.
/// </summary>
public enum ExpiryUrgencyLevel
{
    /// <summary>Certification is valid and beyond the warning window.</summary>
    Valid,

    /// <summary>Certification is approaching expiration within the standard warning window (e.g. 30 days).</summary>
    Warning,

    /// <summary>Certification is approaching expiration imminently (within 7 days).</summary>
    Critical,

    /// <summary>Certification has already expired and is no longer valid.</summary>
    Expired
}

/// <summary>
/// Data transfer representation of an expiring certificate associated with a worker.
/// </summary>
public record WorkerExpiringCertificateInfo(
    Guid WorkerId,
    string WorkerName,
    string BadgeNumber,
    string Trade,
    Guid CertificateId,
    string CertificateName,
    string CertificateNumber,
    DateTime ExpiryDate,
    int DaysUntilExpiry,
    ExpiryUrgencyLevel UrgencyLevel,
    string NotificationMessage
);

/// <summary>
/// Helper utility providing domain logic for calculating certification expiry timelines,
/// checking safety warning windows, formulating HSE alert notifications, and filtering workers.
/// </summary>
public static class CertificationExpiryHelper
{
    /// <summary>
    /// Default warning window in days before a certification expires (30 days).
    /// </summary>
    public const int DefaultWarningDays = 30;

    /// <summary>
    /// Critical warning window in days before expiration (7 days).
    /// </summary>
    public const int CriticalWarningDays = 7;

    /// <summary>
    /// Calculates the number of calendar days remaining until certification expiry.
    /// Returns a negative integer if the certification has already expired.
    /// </summary>
    /// <param name="expiryDate">The expiration date of the certificate.</param>
    /// <param name="referenceDate">
    /// Optional reference date to evaluate against. If null, <see cref="DateTime.UtcNow"/> is used.
    /// </param>
    /// <returns>Days until expiry (positive if valid in future, 0 if expires today, negative if expired).</returns>
    public static int CalculateDaysUntilExpiry(DateTime expiryDate, DateTime? referenceDate = null)
    {
        var baseline = (referenceDate ?? DateTime.UtcNow).Date;
        return (expiryDate.Date - baseline).Days;
    }

    /// <summary>
    /// Determines whether a certificate is currently within the warning window and has not yet expired.
    /// </summary>
    /// <param name="expiryDate">The expiration date of the certificate.</param>
    /// <param name="warningWindowDays">The warning threshold window in days (default 30 days).</param>
    /// <param name="referenceDate">Optional reference date. Defaults to UtcNow.</param>
    /// <returns>True if the certificate expires within the next [0, warningWindowDays] days.</returns>
    public static bool IsWithinWarningWindow(DateTime expiryDate, int warningWindowDays = DefaultWarningDays, DateTime? referenceDate = null)
    {
        var daysRemaining = CalculateDaysUntilExpiry(expiryDate, referenceDate);
        return daysRemaining >= 0 && daysRemaining <= warningWindowDays;
    }

    /// <summary>
    /// Checks whether a certificate has expired relative to the specified reference date.
    /// </summary>
    /// <param name="expiryDate">The certificate expiration date.</param>
    /// <param name="referenceDate">Optional reference date. Defaults to UtcNow.</param>
    /// <returns>True if the certificate has expired.</returns>
    public static bool IsExpired(DateTime expiryDate, DateTime? referenceDate = null)
    {
        return CalculateDaysUntilExpiry(expiryDate, referenceDate) < 0;
    }

    /// <summary>
    /// Determines the urgency level of a certificate based on its expiration date.
    /// </summary>
    /// <param name="expiryDate">The expiration date of the certificate.</param>
    /// <param name="warningWindowDays">The warning window threshold (default 30 days).</param>
    /// <param name="referenceDate">Optional reference date. Defaults to UtcNow.</param>
    /// <returns>The calculated <see cref="ExpiryUrgencyLevel"/>.</returns>
    public static ExpiryUrgencyLevel GetExpiryUrgencyLevel(
        DateTime expiryDate,
        int warningWindowDays = DefaultWarningDays,
        DateTime? referenceDate = null)
    {
        var daysRemaining = CalculateDaysUntilExpiry(expiryDate, referenceDate);

        if (daysRemaining < 0)
        {
            return ExpiryUrgencyLevel.Expired;
        }

        if (daysRemaining <= CriticalWarningDays)
        {
            return ExpiryUrgencyLevel.Critical;
        }

        if (daysRemaining <= warningWindowDays)
        {
            return ExpiryUrgencyLevel.Warning;
        }

        return ExpiryUrgencyLevel.Valid;
    }

    /// <summary>
    /// Formulates a standardized HSE compliance notification message for a worker's certification.
    /// </summary>
    /// <param name="workerName">The worker's full name.</param>
    /// <param name="certificateName">The name or description of the certificate.</param>
    /// <param name="expiryDate">The certificate expiry date.</param>
    /// <param name="referenceDate">Optional reference date. Defaults to UtcNow.</param>
    /// <returns>A formatted notification string tailored to safety compliance officers.</returns>
    public static string GenerateExpiryNotificationMessage(
        string workerName,
        string certificateName,
        DateTime expiryDate,
        DateTime? referenceDate = null)
    {
        var days = CalculateDaysUntilExpiry(expiryDate, referenceDate);
        var dateFormatted = expiryDate.ToString("yyyy-MM-dd");

        if (days < 0)
        {
            var overdueDays = Math.Abs(days);
            return $"[EXPIRED - ACTION REQUIRED] Certification '{certificateName}' for {workerName} expired on {dateFormatted} ({overdueDays} days ago). Worker is restricted from high-hazard activities until recertified.";
        }

        if (days == 0)
        {
            return $"[CRITICAL EXPIRATION TODAY] Certification '{certificateName}' for {workerName} expires today ({dateFormatted}). Work permits requiring this certificate must be suspended tomorrow unless renewed.";
        }

        if (days <= CriticalWarningDays)
        {
            return $"[CRITICAL NOTICE] Certification '{certificateName}' for {workerName} expires in {days} day(s) on {dateFormatted}. Expedite renewal to prevent site access revocation.";
        }

        if (days <= DefaultWarningDays)
        {
            return $"[EXPIRY WARNING] Certification '{certificateName}' for {workerName} expires in {days} days on {dateFormatted}. Please schedule refresher assessment.";
        }

        return $"[COMPLIANT] Certification '{certificateName}' for {workerName} is valid until {dateFormatted} ({days} days remaining).";
    }

    /// <summary>
    /// Filters a list of workers to return only those who possess at least one certificate
    /// expiring within the given threshold days (or already expired).
    /// </summary>
    /// <param name="workers">The collection of workers to inspect.</param>
    /// <param name="daysThreshold">Days threshold window (default 30 days).</param>
    /// <param name="referenceDate">Optional reference date. Defaults to UtcNow.</param>
    /// <returns>Filtered list of workers requiring HSE attention.</returns>
    public static List<Worker> FilterWorkersByUpcomingExpirations(
        IEnumerable<Worker> workers,
        int daysThreshold = DefaultWarningDays,
        DateTime? referenceDate = null)
    {
        ArgumentNullException.ThrowIfNull(workers);

        return workers
            .Where(w => w.IsActive && w.Certificates.Any(c =>
                c.Status != CertificateStatus.Suspended &&
                CalculateDaysUntilExpiry(c.ExpiryDate, referenceDate) <= daysThreshold))
            .ToList();
    }

    /// <summary>
    /// Extracts a flattened list of all certificates nearing expiration across a pool of workers,
    /// complete with urgency levels and pre-generated notification messages.
    /// </summary>
    /// <param name="workers">The collection of workers.</param>
    /// <param name="daysThreshold">The maximum days threshold (default 30 days).</param>
    /// <param name="referenceDate">Optional reference date.</param>
    /// <returns>Sorted list of detailed expiring certificate entries ordered by nearest expiration.</returns>
    public static List<WorkerExpiringCertificateInfo> GetExpiringWorkerCertificates(
        IEnumerable<Worker> workers,
        int daysThreshold = DefaultWarningDays,
        DateTime? referenceDate = null)
    {
        ArgumentNullException.ThrowIfNull(workers);

        var list = new List<WorkerExpiringCertificateInfo>();

        foreach (var worker in workers)
        {
            if (!worker.IsActive) continue;

            var fullName = $"{worker.FirstName} {worker.LastName}".Trim();

            foreach (var cert in worker.Certificates)
            {
                if (cert.Status == CertificateStatus.Suspended) continue;

                var days = CalculateDaysUntilExpiry(cert.ExpiryDate, referenceDate);
                if (days <= daysThreshold)
                {
                    var certName = cert.CertificateType?.Name ?? (!string.IsNullOrWhiteSpace(cert.CertificateNumber) ? cert.CertificateNumber : "Unknown Certification");
                    var urgency = GetExpiryUrgencyLevel(cert.ExpiryDate, daysThreshold, referenceDate);
                    var message = GenerateExpiryNotificationMessage(fullName, certName, cert.ExpiryDate, referenceDate);

                    list.Add(new WorkerExpiringCertificateInfo(
                        worker.Id,
                        fullName,
                        worker.BadgeNumber,
                        worker.Trade,
                        cert.Id,
                        certName,
                        cert.CertificateNumber,
                        cert.ExpiryDate,
                        days,
                        urgency,
                        message
                    ));
                }
            }
        }

        return list.OrderBy(item => item.DaysUntilExpiry).ToList();
    }
}
