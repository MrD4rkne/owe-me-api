# Tests

Below you can find a short description of the tests.

# Test Categories

| Test Type | Location | Purpose | Execution |
|-----------|----------|---------|-----------|
| **Unit Tests** | `*.UnitTests` | Individual component validation | Every build via [build-and-test](#build-and-test) |
| **Architecture Tests** | `Architecture.Tests` | Architectural constraint enforcement | Every build via [build-and-test](#build-and-test) |
| **Smoke Tests** | `SmokeTests` | End-to-end critical path validation | Post-deployment via [run-smoketests](#run-smoketests) |