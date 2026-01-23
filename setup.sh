#!/bin/bash

echo "╔═══════════════════════════════════════════════════╗"
echo "║                                                   ║"
echo "║         🚀 StartUp Template Setup 🚀             ║"
echo "║                                                   ║"
echo "╚═══════════════════════════════════════════════════╝"
echo "
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
"

read -p $'\nEnter your new project name (e.g., MyAwesomeProject): ' NEW_NAME

if [ -z "$NEW_NAME" ]; then
    echo "❌ Project name cannot be empty. Exiting."
    exit 1
fi

OLD_NAME="StartUp"

echo ""
echo "📝 Renaming from '$OLD_NAME' to '$NEW_NAME'..."

# Update file contents
find . -type f \( -name "*.cs" -o -name "*.csproj" -o -name "*.sln" -o -name "*.json" -o -name "*.md" -o -name "*.yml" -o -name "*.yaml" \) \
    -not -path "*/bin/*" -not -path "*/obj/*" -not -path "*/.git/*" -not -path "*/.vs/*" \
    -exec sed -i "s/$OLD_NAME/$NEW_NAME/g" {} +

# Rename files and directories
find . -depth -name "*$OLD_NAME*" -not -path "*/.git/*" | while read -r file; do
    newfile=$(echo "$file" | sed "s/$OLD_NAME/$NEW_NAME/g")
    if [ "$file" != "$newfile" ]; then
        mv "$file" "$newfile" 2>/dev/null
    fi
done

# Delete setup script itself
rm -- "$0"

echo ""
echo "✅ Setup complete! Project renamed to '$NEW_NAME'"
echo ""
echo "📋 Next steps:"
echo "  1. Update appsettings.json with your configuration"
echo "  2. Run: dotnet restore"
echo "  3. Run: dotnet build"
echo "  4. Run: dotnet ef database update"
echo ""
echo "Happy coding! 🎉"