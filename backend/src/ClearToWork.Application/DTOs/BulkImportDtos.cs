using System.ComponentModel.DataAnnotations;

namespace ClearToWork.Application.DTOs;

/// <summary>
/// Request payload for bulk importing workforce personnel records from CSV or Excel sheets.
/// </summary>
/// <param name="Rows">The list of individual worker import rows to process.</param>
public record BulkWorkerImportRequest(
    [Required] List<WorkerImportRow> Rows
)
{
    /// <summary>
    /// Parameterless constructor for JSON deserialization support.
    /// </summary>
    public BulkWorkerImportRequest() : this(new List<WorkerImportRow>()) { }
}

/// <summary>
/// Represents a single worker entry within a bulk import batch.
/// </summary>
/// <param name="Name">Full name of the worker (e.g. "Dinithi Fernando").</param>
/// <param name="Trade">Worker's trade or craft (e.g. "Welder", "Electrician", "Rigger").</param>
/// <param name="CompanyName">Employer contractor company name.</param>
/// <param name="CertificationType">Optional initial certification type code or name (e.g. "HOT_WORK", "BOSIET").</param>
/// <param name="CertExpiryDate">Optional expiration date of the worker's initial certification.</param>
/// <param name="Email">Optional contact email address for the worker.</param>
public record WorkerImportRow(
    [Required(ErrorMessage = "Worker name is required.")]
    string Name,

    [Required(ErrorMessage = "Worker trade is required.")]
    string Trade,

    [Required(ErrorMessage = "Contractor company name is required.")]
    string CompanyName,

    string? CertificationType = null,

    DateTime? CertExpiryDate = null,

    [EmailAddress(ErrorMessage = "Invalid email format.")]
    string? Email = null
)
{
    /// <summary>
    /// Parameterless constructor for serialization and reflection support.
    /// </summary>
    public WorkerImportRow() : this(string.Empty, string.Empty, string.Empty) { }

    /// <summary>
    /// Helper method to split the single <see cref="Name"/> string into First and Last names.
    /// </summary>
    /// <returns>A tuple of (FirstName, LastName).</returns>
    public (string FirstName, string LastName) SplitName()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            return (string.Empty, string.Empty);
        }

        var parts = Name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1)
        {
            return (parts[0], string.Empty);
        }

        var firstName = parts[0];
        var lastName = string.Join(" ", parts.Skip(1));
        return (firstName, lastName);
    }
}

/// <summary>
/// Details of a validation or business logic error encountered during bulk import processing.
/// </summary>
/// <param name="RowNumber">1-based row index in the imported file where the error occurred.</param>
/// <param name="Field">The name of the attribute or column that failed validation.</param>
/// <param name="Message">Descriptive human-readable explanation of why the row failed.</param>
public record ImportError(
    int RowNumber,
    string Field,
    string Message
);

/// <summary>
/// Result summary returned upon completion of a bulk workforce import job.
/// </summary>
/// <param name="SuccessCount">Number of worker profiles successfully parsed and imported into the database.</param>
/// <param name="FailedCount">Number of records that failed validation or insertion.</param>
/// <param name="Errors">Detailed list of individual row-level errors.</param>
public record BulkImportResult(
    int SuccessCount,
    int FailedCount,
    List<ImportError> Errors
)
{
    /// <summary>
    /// Total number of rows evaluated in the import request.
    /// </summary>
    public int TotalProcessed => SuccessCount + FailedCount;

    /// <summary>
    /// Indicates whether the entire batch succeeded with zero failures.
    /// </summary>
    public bool IsSuccess => FailedCount == 0;

    /// <summary>
    /// Parameterless constructor for serialization frameworks.
    /// </summary>
    public BulkImportResult() : this(0, 0, new List<ImportError>()) { }

    /// <summary>
    /// Creates a successful import result.
    /// </summary>
    public static BulkImportResult Succeeded(int count) => new(count, 0, new List<ImportError>());

    /// <summary>
    /// Creates a partial or failed import result.
    /// </summary>
    public static BulkImportResult WithErrors(int successCount, List<ImportError> errors) =>
        new(successCount, errors.Count, errors);
}
