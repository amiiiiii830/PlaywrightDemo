
dotnet new console -n PlaywrightDemo
cd PlaywrightDemo

# Add project dependency
dotnet add package Microsoft.Playwright
# Build the project
dotnet build



  
  dotnet tool install --global Microsoft.Playwright.CLI

  cat << \EOF >> ~/.bashrc
  # Add .NET Core SDK tools
  export PATH="$PATH:/home/jaykchen/.dotnet/tools"
  EOF
  export PATH="$PATH:/home/jaykchen/.dotnet/tools"
  mkdir -p bin/Debug/net8.0/.playwright/unix/native

  sudo nano ~/.bashrc
  source  ~/.bashrc
  playwright install

  dotnet add package Microsoft.Playwright
  
  dotnet tool list -g
  dotnet tool uninstall --global Microsoft.Playwright.CLI
  dotnet tool install --global Microsoft.Playwright.CLI