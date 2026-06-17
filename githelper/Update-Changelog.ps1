param(
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [string]$Date = (Get-Date -Format "yyyy-MM-dd"),

    [switch]$DryRun
)

$ErrorActionPreference = "Stop"

$normalizedVersion = $Version.TrimStart("v")
if ($normalizedVersion -notmatch "^\d+\.\d+\.\d+$") {
    throw "Version must match MAJOR.MINOR.PATCH, for example 1.2.3 or v1.2.3."
}

$repoRoot = Split-Path -Parent $PSScriptRoot
$changelogPath = Join-Path $repoRoot "CHANGELOG.md"

if (-not (Test-Path $changelogPath)) {
    throw "CHANGELOG.md not found at $changelogPath."
}

$content = Get-Content $changelogPath -Raw
$releaseHeading = "## [$normalizedVersion] - $Date"

if ($content -match "(?m)^## \[$([regex]::Escape($normalizedVersion))\]") {
    throw "CHANGELOG.md already contains a section for version $normalizedVersion."
}

$unreleasedPattern = "(?ms)^## \[Unreleased\]\s*(?<body>.*?)(?=^## \[|\z)"
$match = [regex]::Match($content, $unreleasedPattern)
if (-not $match.Success) {
    throw "CHANGELOG.md must contain a '## [Unreleased]' section."
}

$body = $match.Groups["body"].Value.Trim()
if ([string]::IsNullOrWhiteSpace($body)) {
    throw "The '## [Unreleased]' section is empty."
}

$replacement = "## [Unreleased]`r`n`r`n$releaseHeading`r`n`r`n$body`r`n`r`n"
$updated = [regex]::Replace($content, $unreleasedPattern, $replacement, 1)

if ($DryRun) {
    $updated
    exit 0
}

Set-Content -Path $changelogPath -Value $updated -NoNewline -Encoding UTF8
