# GameTradeZone

## Tech Stack

### Frontend
- Vue 3 + TypeScript
- Vite
- Pinia (State Management)
- Vue Router
- Axios

### Backend
- ASP.NET Core Web API
- Entity Framework Core
- SignalR (Realtime Communication)
- JWT Authentication

### Database
- SQL Server

### Integrations
- Cloudinary (Image Upload)
- Sepay (Payment Integration)

---

## Project Structure

GameTradeZone/
│
├── FrontEnd/                 # User client (Vue 3)
├── Admin/                    # Admin dashboard (Vue 3)
├── Backend/
│   ├── GameTradeZone/                # API Layer (Controllers)
│   ├── GameTradeZone.Domain/         # Entities, Constants
│   ├── GameTradeZone.Infrastructure/ # DbContext, Migrations
│   └── GameTradeZone.Service/        # Business Logic Layer

---

## Core Features

### Authentication & User Management
- JWT-based authentication
- External login support (OAuth)
- Profile management
- Role-based authorization

### Game Account Marketplace
- List and manage game accounts
- Buy/sell accounts
- Upload images and metadata
- Account verification system

### Auction System
- Create auction sessions
- Real-time bidding using SignalR
- Automatic winner selection

### Service Marketplace
- Post gaming services
- Hire and manage service progress
- Confirm service completion

### Payment System
- Deposit via bank integration
- Withdraw requests
- Transaction history tracking

### Dispute Management
- Create disputes between users
- Admin moderation and resolution

### Admin Dashboard
- Manage users and roles
- Manage game data
- Monitor transactions
- Handle disputes and reports
- View system statistics

---

## Setup & Installation

1. Clone Repository

git clone https://github.com/your-repo/GameTradeZone.git
cd GameTradeZone

2. Backend Setup (.NET)

cd Backend/GameTradeZone
dotnet restore
dotnet ef database update
dotnet run

API will run at:
https://localhost:7232

3. Frontend Setup (User)

cd FrontEnd
npm install
npm run dev

4. Admin Dashboard

cd Admin
npm install
npm run dev

---

## Environment Configuration

Backend (appsettings.json)

{
  "ConnectionStrings": {
    "DefaultConnection": "Your SQL Server connection string"
  },
  "Jwt": {
    "Key": "your-secret-key",
    "Issuer": "your-app",
    "Audience": "your-app"
  }
}

---

## API Overview

Base URL:
/api/

Main Endpoints:
- /Authenticate
- /AccountGame
- /Auction
- /Service
- /Dispute
- /RechargeBank
- /WebsiteAccount

---

## Realtime Communication

Implemented using SignalR

Used for:
- Auction bidding updates
- Notifications

Hub endpoint:
/hubs/auction

---

## Architecture

- Domain Layer: Entities and core business models
- Infrastructure Layer: Database and persistence logic
- Service Layer: Business logic and application services
- API Layer: Controllers and endpoints

Key principles:
- Separation of concerns
- Modular service design
- Scalable structure for future expansion

---

## Project Status

This project was developed under time constraints, therefore some parts of the codebase may not fully follow best practices.
