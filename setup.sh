#!/bin/bash
# setup-minimal.sh - أبسط نسخة للـ Linux/Mac

echo "Starting setup..."

# Remove old .git folder completely
if [ -d ".git" ]; then
    rm -rf .git
    echo "Removed old .git folder"
fi


# Initialize fresh git repository
git init
git add .
git commit -m "Initial commit"

# Remove any remotes if they exist
for remote in $(git remote); do
    git remote remove "$remote"
done

# Restore and build
dotnet restore
dotnet build

echo "Done!"