# setup-minimal.ps1 - أبسط نسخة
Write-Host "Starting setup..." -ForegroundColor Green

# Remove old .git folder completely
if (Test-Path ".git") {
    Remove-Item ".git" -Recurse -Force
    Write-Host "Removed old .git folder" -ForegroundColor Yellow
}


# Initialize fresh git repository
git init
git add .
git commit -m "Initial commit"

# Remove any remotes if they exist
$remotes = git remote
foreach ($remote in $remotes) {
    git remote remove $remote
}

# Restore and build
dotnet restore
dotnet build

Write-Host "Done!" -ForegroundColor Green