Write-Host @"
╔═══════════════════════════════════════════════════╗
║                                                   ║
║         🚀 StartUp Template Setup 🚀             ║
║                                                   ║
╔═══════════════════════════════════════════════════╝
                ▬▬▬.◙.▬▬▬
                ▂▄▄▓▄▄▂
                ◢◤ █▀▀████▄▄▄▄__◢◤
                █▄▂█ █▄███▀▀▀▀▀▀▀╬
                ◥█████◤
                ══╩══╩══
                ╬═╬
                ╬═╬
                ╬═╬
                ╬═╬
                ╬═╬
                ╬═╬☻/
                ╬═╬/▌
                ╬═╬//
"@ -ForegroundColor Cyan

$NewName = Read-Host "`nEnter your new project name (e.g., MyAwesomeProject)"

if ([string]::IsNullOrWhiteSpace($NewName)) {
    Write-Host "❌ Project name cannot be empty. Exiting." -ForegroundColor Red
    exit 1
}

$OldName = "StartUp"

Write-Host "`n📝 Renaming from '$OldName' to '$NewName'..." -ForegroundColor Yellow

# Update file contents
$fileTypes = @("*.cs", "*.csproj", "*.sln", "*.json", "*.xml", "*.config", "*.md", "*.yml", "*.yaml")
$excludeDirs = @("bin", "obj", ".vs", ".git")

Get-ChildItem -Recurse -File -Include $fileTypes | 
    Where-Object { 
        $exclude = $false
        foreach ($dir in $excludeDirs) {
            if ($_.FullName -like "*\$dir\*") {
                $exclude = $true
                break
            }
        }
        -not $exclude
    } | ForEach-Object {
    $content = Get-Content $_.FullName -Raw -ErrorAction SilentlyContinue
    if ($content -and $content -match [regex]::Escape($OldName)) {
        $newContent = $content -replace [regex]::Escape($OldName), $NewName
        Set-Content $_.FullName -Value $newContent -NoNewline
    }
}

# Rename files
Get-ChildItem -Recurse -File | 
    Where-Object { $_.Name -like "*$OldName*" } | 
    ForEach-Object {
    $newFileName = $_.Name -replace [regex]::Escape($OldName), $NewName
    Rename-Item $_.FullName -NewName $newFileName -ErrorAction SilentlyContinue
}

# Rename directories (deepest first)
Get-ChildItem -Recurse -Directory | 
    Where-Object { $_.Name -like "*$OldName*" } |
    Sort-Object { $_.FullName.Length } -Descending |
    ForEach-Object {
    $newDirName = $_.Name -replace [regex]::Escape($OldName), $NewName
    Rename-Item $_.FullName -NewName $newDirName -ErrorAction SilentlyContinue
}

# Delete setup script itself
Remove-Item $PSCommandPath -Force

Write-Host "`n✅ Setup complete! Project renamed to '$NewName'" -ForegroundColor Green
Write-Host @"

📋 Next steps:
  1. Update appsettings.json with your configuration
  2. Run: dotnet restore
  3. Run: dotnet build  
  4. Run: dotnet ef database update

Happy coding! 🎉
"@ -ForegroundColor Cyan