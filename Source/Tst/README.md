# Tests folder

This document covers **local `dotnet test`** workflows (standard vs Explicit) and **Linux Docker** runs.

Unless noted otherwise, run commands from the **`Source`** directory (the folder that contains `KZDev.PerfUtils.sln`). The solution uses **artifacts output** (`UseArtifactsOutput`); test build outputs live under **`artifacts/bin/<ProjectName>/release_net*`** at the repository root, not under each project’s `bin` folder.

---

## Local: standard full suite (Release)

Restores, builds, and runs **non-Explicit** tests with the defaults wired in `Directory.Build.props` / `Directory.Build.targets` (`xunit.runner.json`, `KZDev.PerfUtils.tests.runsettings`).

**PowerShell or cmd (from `Source`):**

```cmd
dotnet restore KZDev.PerfUtils.sln
dotnet build KZDev.PerfUtils.sln -c Release
dotnet test KZDev.PerfUtils.sln -c Release --no-build
```

**bash (from `Source`):**

```bash
dotnet restore KZDev.PerfUtils.sln
dotnet build KZDev.PerfUtils.sln -c Release
dotnet test KZDev.PerfUtils.sln -c Release --no-build
```

`[Fact(Explicit = true)]` tests are **skipped** in this default configuration.

---

## Local: Explicit-only run (serialized xUnit)

Explicit stress tests are marked with **`[Trait(TestMode, Explicit)]`** (see `KZDev.PerfUtils.TestBase/TestConstants.cs`: trait name **`TestMode`**, value **`Explicit`**). Prefer **trait-based** selection so shared test sources are not tied to fragile method-name lists.

This repository’s `dotnet test` path uses **VSTest** (not Microsoft Testing Platform by default). Use a VSTest **`--filter`** for the trait, and pass **xUnit RunSettings** after `--` so explicit tests **execute** and xUnit runs **single-threaded** with **no assembly/collection parallelism** (same values as `Tst/xunit.runner.explicit.json`).

**One-shot command (from `Source`, after a Release build):**

```cmd
dotnet test KZDev.PerfUtils.sln -c Release --no-build --filter "TestMode=Explicit" -- xUnit.Explicit=only xUnit.MaxParallelThreads=1 xUnit.ParallelizeAssembly=false xUnit.ParallelizeTestCollections=false
```

**bash** (keep the filter expression in quotes so the shell does not treat `=` specially):

```bash
dotnet test KZDev.PerfUtils.sln -c Release --no-build --filter "TestMode=Explicit" -- xUnit.Explicit=only xUnit.MaxParallelThreads=1 xUnit.ParallelizeAssembly=false xUnit.ParallelizeTestCollections=false
```

**Notes:**

- Do **not** pass xUnit query filters such as `--filter-query /[TestMode=Explicit]` after `--` with this VSTest setup; the leading `/` is treated like a runsettings token and triggers **“invalid token”** errors. Prefer the `--filter` + `xUnit.*` form above.
- **CI parity (optional):** To match the workflow that copies `Tst/xunit.runner.explicit.json` over each test output `xunit.runner.json`, run a **Release** `dotnet build` first, then copy that file onto every `artifacts/bin/*/release_net*/xunit.runner.json` under the repo root, then run the same `dotnet test` line **including** `--filter "TestMode=Explicit"` and `-- xUnit.Explicit=only` (the copy adjusts parallelism; the filter + `Explicit=only` still selects and runs explicit tests).

**Optional stricter host parallelism:** To cap VSTest process parallelism as well, copy `KZDev.PerfUtils.tests.runsettings` to a local file, set `<MaxCpuCount>1</MaxCpuCount>` under `<RunConfiguration>`, and add `dotnet test ... --settings path\to\your.runsettings`. Session timeout and other settings can stay aligned with the checked-in runsettings.

---

## Trait convention for new Explicit tests

Add **`[Trait(TestConstants.TestTrait.TestMode, TestConstants.TestMode.Explicit)]`** (or the string literals **`TestMode`** / **`Explicit`**) alongside **`[Fact(Explicit = true)]`** so Explicit-only runs remain discovery-based across assemblies that share source files.

---

## Maintainers: branch protection (CI)

Configure GitHub **branch protection** status checks to match how CI gates merges:

- **`main`:** Require **`test-standard`** and **`test-explicit`** from the **CI** workflow so both jobs must pass before merge.
- **`dev`:** Require **`test-standard`** only. **`test-explicit`** is informational on `dev` (workflow uses `continue-on-error` for pushes to `dev` and for pull requests whose base branch is not `main`).

Fork pull requests need repository settings that allow GitHub Actions to run from forks if you want CI for external contributors.

---

## Linux: Docker-based test run

> Run these commands from the \{RepositoryRoot\}\Source directory

### Build the image from the Dockerfile

```cmd
docker build -t perfutils-linux-tests -f linuxtest.dockerfile .
```

### Create a host machine folder for the test results

This folder will be used to store the test results from the container so you can review them later.

```cmd
mkdir {/path/to/host/machine/folder}
```

> Replace _\{/path/to/host/machine/folder\}_ with the path to the folder on the host machine you want to store the test result files in.

### Run the container from the image

This will run the tests and store the results in the folder you created in the previous step.

```cmd
docker run --rm -v {/path/to/host/machine/folder}://app/Tst/linux-tests-results perfutils-linux-tests
```

### Review the test results

The test result files are stored in the folder you specified in the previous step (\{/path/to/host/machine/folder\}) in xUnit XML format.

### Cleanup the image when done with tests

Keep your environment clean by removing the image when you are done running tests.

```cmd
docker rmi perfutils-linux-tests
```
