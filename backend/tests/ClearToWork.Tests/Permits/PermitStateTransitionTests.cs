using Xunit;

namespace ClearToWork.Tests.Permits;

#region Permit State Machine Domain Definitions

/// <summary>
/// Core lifecycle states for an industrial Permit-to-Work (PTW) in ClearToWork AI.
/// </summary>
public enum PermitState
{
    Draft = 1,
    Submitted = 2,
    UnderReview = 3,
    Approved = 4,
    Rejected = 5,
    Active = 6,
    Suspended = 7,
    Closed = 8
}

/// <summary>
/// Exception thrown when an illegal or unauthorized permit state transition is attempted.
/// </summary>
public class InvalidStateTransitionException : InvalidOperationException
{
    public PermitState CurrentState { get; }
    public PermitState TargetState { get; }

    public InvalidStateTransitionException(PermitState currentState, PermitState targetState, string? message = null)
        : base(message ?? $"Illegal permit state transition: Cannot transition from '{currentState}' to '{targetState}'.")
    {
        CurrentState = currentState;
        TargetState = targetState;
    }
}

/// <summary>
/// State machine engine enforcing safe lifecycle transitions for permits in oil &amp; gas operations.
/// Prevents bypassing safety checks, premature activations, and illegal modifications.
/// </summary>
public class PermitStateMachine
{
    private static readonly Dictionary<PermitState, HashSet<PermitState>> AllowedTransitions = new()
    {
        [PermitState.Draft] = new HashSet<PermitState> { PermitState.Submitted },
        [PermitState.Submitted] = new HashSet<PermitState> { PermitState.UnderReview, PermitState.Draft },
        [PermitState.UnderReview] = new HashSet<PermitState> { PermitState.Approved, PermitState.Rejected, PermitState.Draft },
        [PermitState.Approved] = new HashSet<PermitState> { PermitState.Active, PermitState.Suspended },
        [PermitState.Active] = new HashSet<PermitState> { PermitState.Suspended, PermitState.Closed },
        [PermitState.Suspended] = new HashSet<PermitState> { PermitState.Active, PermitState.Closed },
        [PermitState.Rejected] = new HashSet<PermitState> { PermitState.Draft }, // Can be reopened as draft for corrections
        [PermitState.Closed] = new HashSet<PermitState>() // Terminal state - immutable record
    };

    public PermitState CurrentState { get; private set; }
    public DateTime? SubmittedAt { get; private set; }
    public DateTime? UnderReviewAt { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime? ActivatedAt { get; private set; }
    public DateTime? SuspendedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }
    public string? RejectionReason { get; private set; }
    public string? SuspensionReason { get; private set; }
    public List<string> TransitionAuditTrail { get; } = new();

    public PermitStateMachine(PermitState initialState = PermitState.Draft)
    {
        CurrentState = initialState;
        TransitionAuditTrail.Add($"Initialized in state: {initialState} at {DateTime.UtcNow:O}");
    }

    /// <summary>
    /// Checks whether a proposed transition from one state to another is permitted by safety rules.
    /// </summary>
    public static bool CanTransition(PermitState from, PermitState to)
    {
        return AllowedTransitions.TryGetValue(from, out var allowed) && allowed.Contains(to);
    }

    /// <summary>
    /// Executes a state transition. Throws <see cref="InvalidStateTransitionException"/> if invalid.
    /// </summary>
    public void TransitionTo(PermitState targetState, string? reason = null)
    {
        if (!CanTransition(CurrentState, targetState))
        {
            throw new InvalidStateTransitionException(CurrentState, targetState);
        }

        var previousState = CurrentState;
        CurrentState = targetState;
        var timestamp = DateTime.UtcNow;

        switch (targetState)
        {
            case PermitState.Submitted:
                SubmittedAt = timestamp;
                break;
            case PermitState.UnderReview:
                UnderReviewAt = timestamp;
                break;
            case PermitState.Approved:
                ApprovedAt = timestamp;
                break;
            case PermitState.Rejected:
                RejectionReason = reason;
                break;
            case PermitState.Active:
                ActivatedAt = timestamp;
                break;
            case PermitState.Suspended:
                SuspendedAt = timestamp;
                SuspensionReason = reason;
                break;
            case PermitState.Closed:
                ClosedAt = timestamp;
                break;
        }

        TransitionAuditTrail.Add($"Transitioned {previousState} -> {targetState} at {timestamp:O}. Reason: {reason ?? "N/A"}");
    }
}

#endregion

/// <summary>
/// Unit test suite verifying all valid and invalid state transitions of the Permit State Machine.
/// </summary>
public class PermitStateTransitionTests
{
    [Fact]
    public void Transition_DraftToSubmitted_SucceedsAndSetsTimestamp()
    {
        // Arrange
        var sm = new PermitStateMachine(PermitState.Draft);

        // Act
        sm.TransitionTo(PermitState.Submitted, "Supervisor submitted permit for multi-agent check");

        // Assert
        Assert.Equal(PermitState.Submitted, sm.CurrentState);
        Assert.NotNull(sm.SubmittedAt);
        Assert.Contains(sm.TransitionAuditTrail, t => t.Contains("Draft -> Submitted"));
    }

    [Fact]
    public void Transition_SubmittedToUnderReview_SucceedsAndRecordsReviewState()
    {
        // Arrange
        var sm = new PermitStateMachine(PermitState.Draft);
        sm.TransitionTo(PermitState.Submitted);

        // Act
        sm.TransitionTo(PermitState.UnderReview, "AI verification complete; Safety Officer reviewing");

        // Assert
        Assert.Equal(PermitState.UnderReview, sm.CurrentState);
        Assert.NotNull(sm.UnderReviewAt);
    }

    [Fact]
    public void Transition_UnderReviewToApproved_WhenPrerequisitesMet_Succeeds()
    {
        // Arrange
        var sm = new PermitStateMachine(PermitState.Draft);
        sm.TransitionTo(PermitState.Submitted);
        sm.TransitionTo(PermitState.UnderReview);

        // Act
        sm.TransitionTo(PermitState.Approved, "All isolation tags verified and HSE officer signed off");

        // Assert
        Assert.Equal(PermitState.Approved, sm.CurrentState);
        Assert.NotNull(sm.ApprovedAt);
    }

    [Fact]
    public void Transition_UnderReviewToRejected_WithReason_SucceedsAndStoresReason()
    {
        // Arrange
        var sm = new PermitStateMachine(PermitState.Draft);
        sm.TransitionTo(PermitState.Submitted);
        sm.TransitionTo(PermitState.UnderReview);

        const string rejectionReason = "SIMOPS conflict: Hot work overlapping with hydro-testing in Zone C";

        // Act
        sm.TransitionTo(PermitState.Rejected, rejectionReason);

        // Assert
        Assert.Equal(PermitState.Rejected, sm.CurrentState);
        Assert.Equal(rejectionReason, sm.RejectionReason);
    }

    [Fact]
    public void Transition_ApprovedToActive_OnSiteVerification_Succeeds()
    {
        // Arrange
        var sm = new PermitStateMachine(PermitState.Draft);
        sm.TransitionTo(PermitState.Submitted);
        sm.TransitionTo(PermitState.UnderReview);
        sm.TransitionTo(PermitState.Approved);

        // Act
        sm.TransitionTo(PermitState.Active, "Physical board QR scanned and GPS location matched zone");

        // Assert
        Assert.Equal(PermitState.Active, sm.CurrentState);
        Assert.NotNull(sm.ActivatedAt);
    }

    [Fact]
    public void Transition_ActiveToSuspended_WhenAdverseConditionsArise_Succeeds()
    {
        // Arrange
        var sm = new PermitStateMachine(PermitState.Draft);
        sm.TransitionTo(PermitState.Submitted);
        sm.TransitionTo(PermitState.UnderReview);
        sm.TransitionTo(PermitState.Approved);
        sm.TransitionTo(PermitState.Active);

        const string suspensionReason = "High wind gusts (>42 km/h) detected on offshore platform";

        // Act
        sm.TransitionTo(PermitState.Suspended, suspensionReason);

        // Assert
        Assert.Equal(PermitState.Suspended, sm.CurrentState);
        Assert.NotNull(sm.SuspendedAt);
        Assert.Equal(suspensionReason, sm.SuspensionReason);
    }

    [Fact]
    public void Transition_SuspendedToActive_WhenHazardsCleared_ReactivatesWork()
    {
        // Arrange: Permit suspended due to temporary weather/gas alert
        var sm = new PermitStateMachine(PermitState.Draft);
        sm.TransitionTo(PermitState.Submitted);
        sm.TransitionTo(PermitState.UnderReview);
        sm.TransitionTo(PermitState.Approved);
        sm.TransitionTo(PermitState.Active);
        sm.TransitionTo(PermitState.Suspended, "Atmospheric gas detection alert");

        // Act: Gas cleared, re-tested safe, reactivate
        sm.TransitionTo(PermitState.Active, "Gas monitoring re-cleared: 0.0% LEL, 20.9% O2");

        // Assert
        Assert.Equal(PermitState.Active, sm.CurrentState);
    }

    [Fact]
    public void Transition_ActiveToClosed_UponShiftCompletion_ClosesPermit()
    {
        // Arrange
        var sm = new PermitStateMachine(PermitState.Draft);
        sm.TransitionTo(PermitState.Submitted);
        sm.TransitionTo(PermitState.UnderReview);
        sm.TransitionTo(PermitState.Approved);
        sm.TransitionTo(PermitState.Active);

        // Act
        sm.TransitionTo(PermitState.Closed, "Work completed, tools removed, housekeeping inspected");

        // Assert
        Assert.Equal(PermitState.Closed, sm.CurrentState);
        Assert.NotNull(sm.ClosedAt);
    }

    [Fact]
    public void Transition_DraftToActive_WithoutApproval_ThrowsInvalidStateTransitionException()
    {
        // Arrange: A bypass attempt from Draft directly to Active
        var sm = new PermitStateMachine(PermitState.Draft);

        // Act & Assert
        var ex = Assert.Throws<InvalidStateTransitionException>(() =>
        {
            sm.TransitionTo(PermitState.Active, "Attempting unauthorized activation");
        });

        Assert.Equal(PermitState.Draft, ex.CurrentState);
        Assert.Equal(PermitState.Active, ex.TargetState);
        Assert.Equal(PermitState.Draft, sm.CurrentState); // State remained unchanged
    }

    [Fact]
    public void Transition_ClosedToAnyState_ThrowsInvalidStateTransitionException()
    {
        // Arrange: Closed permits are immutable legal records
        var sm = new PermitStateMachine(PermitState.Draft);
        sm.TransitionTo(PermitState.Submitted);
        sm.TransitionTo(PermitState.UnderReview);
        sm.TransitionTo(PermitState.Approved);
        sm.TransitionTo(PermitState.Active);
        sm.TransitionTo(PermitState.Closed);

        // Act & Assert
        Assert.Throws<InvalidStateTransitionException>(() => sm.TransitionTo(PermitState.Active));
        Assert.Throws<InvalidStateTransitionException>(() => sm.TransitionTo(PermitState.Draft));
        Assert.Throws<InvalidStateTransitionException>(() => sm.TransitionTo(PermitState.Approved));
        Assert.Equal(PermitState.Closed, sm.CurrentState);
    }
}
