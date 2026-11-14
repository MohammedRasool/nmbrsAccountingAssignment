using NmbrsAccounting.Tests.Pages;
using Reqnroll;

namespace NmbrsAccounting.Tests.StepDefinitions;

/// <summary>
/// Step definitions for the Privacy.feature file.
/// Currently uses steps from NavigationSteps for navigation and title verification.
/// 
/// This class is reserved for future Privacy-specific steps if needed, maintaining
/// consistency with the BDD pattern where each feature has its own step definition class.
/// </summary>
[Binding]
public sealed class PrivacySteps
{
    private readonly PrivacyPage _privacy;

    // Constructor receives PrivacyPage via Reqnroll's dependency injection
    public PrivacySteps(PrivacyPage privacy) => _privacy = privacy;
}
