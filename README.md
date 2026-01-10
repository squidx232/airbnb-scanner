# Airbnb Scanner

A web application that searches Airbnb listings by keywords found in their **full descriptions**, not just titles. Built with .NET 8, Blazor WebAssembly, and Tailwind CSS.

🌐 **Live Demo**: [https://airbnb-scanner.netlify.app](https://airbnb-scanner.netlify.app)

## Features

- **Full-Text Search**: Searches complete listing descriptions, amenities, host info, and house rules
- **Multiple Keywords**: Search with multiple keywords, filter by minimum keyword matches
- **Smart Caching**: Skips previously searched listings to save API credits
- **Advanced Filters**: Property type, bedrooms, beds, bathrooms, price range, amenities
- **Modern UI**: Clean, responsive design with custom date picker
- **Real-Time Progress**: Live search progress with detailed status updates
- **Results Ranking**: Results sorted by number of matched keywords
- **Static Hosting**: Runs entirely in the browser - no server required!

## Screenshots

<img width="1241" height="525" alt="image" src="https://github.com/user-attachments/assets/69510880-69e7-49f3-942e-2b6a525ce48c" />

<img width="872" height="786" alt="image" src="https://github.com/user-attachments/assets/5d192271-528f-411d-be53-5d1f6cd4f8fe" />

<img width="1234" height="438" alt="image" src="https://github.com/user-attachments/assets/3c6aa3ba-19b1-48aa-b70b-e4a52d5908a8" />


## Requirements

For local development:
- .NET 8.0 SDK or later
- SearchAPI.io API key (free tier available)

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/squidx232/airbnb-scanner.git
cd airbnb-scanner
```

### 2. Get an API Key

1. Visit [SearchAPI.io](https://www.searchapi.io/)
2. Create a free account
3. Copy your API key

### 3. Run the application

```bash
cd src/AirbnbKeywordFinder.Web
dotnet run
```

The application will open in your browser at `http://localhost:5000`

### 4. Enter your API key

1. Click "Show Advanced Filters"
2. Scroll to "API Configuration"
3. Paste your API key

## Building for Production

```bash
dotnet publish src/AirbnbKeywordFinder.Web/AirbnbKeywordFinder.Web.csproj -c Release -o ./publish
```

The published output will be in `./publish/wwwroot/` - this folder contains all static files ready for deployment.

## Deploying to Netlify

This app is designed to run on static hosting services like Netlify.

### Option 1: Deploy via GitHub

1. Push your code to GitHub
2. Connect your repo to Netlify
3. Configure build settings:
   - **Build command**: `dotnet publish src/AirbnbKeywordFinder.Web/AirbnbKeywordFinder.Web.csproj -c Release -o output`
   - **Publish directory**: `output/wwwroot`

### Option 2: Manual Deploy

1. Build locally: `dotnet publish src/AirbnbKeywordFinder.Web/AirbnbKeywordFinder.Web.csproj -c Release -o ./publish`
2. Drag and drop the `publish/wwwroot` folder to Netlify

## Project Structure

```
airbnb-scanner/
├── src/
│   ├── AirbnbKeywordFinder.Core/     # Shared library
│   │   ├── Models/                   # Data models
│   │   └── Services/                 # API client, search service
│   └── AirbnbKeywordFinder.Web/      # Blazor WebAssembly application
│       ├── Components/               # Razor components
│       └── wwwroot/                  # Static files (index.html, _redirects)
├── AirbnbKeywordFinder.sln           # Solution file
└── README.md
```

## How It Works

1. **Search Phase**: Queries Airbnb listings for a location using the SearchAPI
2. **Detail Fetch**: For each listing, fetches full property details (description, amenities, etc.)
3. **Keyword Match**: Searches the complete text for your keywords
4. **Smart Cache**: Stores searched property IDs to avoid duplicate API calls
5. **Results**: Displays matches sorted by keyword count (most matches first)

> **Note**: This is a client-side application. All API calls are made directly from your browser using your own API key. Your API key is never sent to any server other than SearchAPI.io.

## Configuration Options

| Option | Description |
|--------|-------------|
| Location | City or area to search |
| Keywords | Comma-separated list of keywords to find |
| Check-in/out | Optional date range |
| Guests | Number of guests (1-16) |
| Currency | Price display currency |
| Property Type | Entire home, private room, or any |
| Bedrooms/Beds/Bathrooms | Minimum room counts |
| Price Range | Min/max price per night |
| Amenities | Required amenities filter |
| Min Keywords | Minimum number of keywords to match |
| Match ALL | Require all keywords to match |
| Demo Mode | Test without using API credits |

## Cache Management

The application caches searched property IDs in your browser to save API credits. If you search the same location twice, previously checked listings will be skipped.

To re-scan listings:
1. Click "Show Advanced Filters"
2. Scroll to "Cache Management"
3. Click "Clear Cache"

## API Credit Usage

Each search uses approximately:
- 1 credit for the search query
- 1 credit per property for full details

Example: Searching 10 properties uses ~11 credits.

## Tech Stack

- .NET 8.0
- Blazor WebAssembly
- Tailwind CSS (via CDN)
- SearchAPI.io (Airbnb API)

## License

MIT License

## Author

Dev by [@whereishassan](https://linkedin.com/in/whereishassan)
