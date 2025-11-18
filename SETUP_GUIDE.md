# Quick Setup Guide

## Step 1: Install .NET 8 SDK

### Windows
1. Go to: https://dotnet.microsoft.com/download/dotnet/8.0
2. Download **.NET 8 SDK** (not Runtime)
3. Run the installer
4. **Restart your PowerShell/Command Prompt**

### Verify Installation
Open a new PowerShell window and run:
```powershell
dotnet --version
```
You should see: `8.0.x` or higher

---

## Step 2: Install SQL Server LocalDB (if needed)

### Option A: If you have Visual Studio
- LocalDB is already included, skip this step

### Option B: Install SQL Server Express
1. Download from: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
2. Choose "Express" edition (free)
3. During installation, select "LocalDB" feature

---

## Step 3: Run the Application

### Open PowerShell in the project root:
```powershell
cd "D:\pricing engine\SmartInventory.Api"
```

### Restore NuGet packages:
```powershell
dotnet restore
```

### Run the application:
```powershell
dotnet run
```

You should see output like:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
      Now listening on: https://localhost:7000
```

---

## Step 4: Access the Application

### Option A: Use the Frontend
1. Open `frontend/index.html` in your browser
2. Or serve it using a local web server:
   ```powershell
   cd ..\frontend
   python -m http.server 8080
   ```
3. Open browser to: `http://localhost:8080`

### Option B: Use Swagger UI
- Open: `https://localhost:7000/swagger` (or `http://localhost:5000/swagger`)

---

## Troubleshooting

### "No .NET SDKs were found"
- **Solution**: Install .NET 8 SDK (see Step 1 above)
- Make sure to restart your terminal after installation

### "Cannot connect to database"
- **Solution**: Ensure SQL Server LocalDB is installed
- Check connection string in `appsettings.json`

### CORS errors in browser
- **Solution**: Update CORS origins in `Program.cs` to match your frontend URL
- Or update `API_BASE_URL` in `frontend/script.js` to match your API URL

### Port already in use
- **Solution**: Change ports in `Properties/launchSettings.json`
- Or kill the process using the port

---

## Need Help?

- .NET Documentation: https://docs.microsoft.com/dotnet
- .NET 8 Download: https://dotnet.microsoft.com/download/dotnet/8.0
- SQL Server Downloads: https://www.microsoft.com/en-us/sql-server/sql-server-downloads

