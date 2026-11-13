using NUnit.Framework;

// Runs the feature files in parallel to keep pipeline time short.
// Safe because every scenario gets its own isolated browser context (see PlaywrightHooks).
[assembly: Parallelizable(ParallelScope.Fixtures)]