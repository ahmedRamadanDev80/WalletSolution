# 💰 Wallet Demo System (.NET 8 + React + MUI)

This project is a **demo wallet management system** built using **ASP.NET Core 8 (Clean Architecture)** on the backend and **React (TypeScript + Material UI)** on the frontend.  
It demonstrates a complete workflow of **earning / burning points**, **tracking wallet balance**, and **viewing transactions**, with **mock JWT authentication**.

---

## ⚙️ Architecture

**Backend:**
- **ASP.NET Core 8**
- **Entity Framework Core 8**
- **SQL Server**
- **Repository Pattern**
- **JWT Authentication (Mock)**
- **Clean layering**
  ```
  src/
  ├── Wallet.Api              → API Controllers + Auth + Swagger
  ├── Wallet.Application      → Services + DTOs
  ├── Wallet.Core             → Entities + Interfaces
  └── Wallet.Infrastructure   → EF DbContext + Repositories + Seed
  ```

**Frontend:**
- **React + TypeScript + Vite**
- **Material UI (MUI 5)**
- **Axios with JWT Interceptor**
- **React Router v6**
- **Modular Pages:** `LoginPage`, `WalletPage`, `TransactionsPage`

---

## 🚀 Running Locally

### 1️⃣ Backend
**Requirements:**  
- .NET 8 SDK  
- SQL Server instance (local or container)

**Setup:**
```bash
cd src/Wallet.Api
dotnet ef database update
dotnet run
```

Backend will start at:  
👉 http://localhost:5000 (Swagger available)

---

### 2️⃣ Frontend
**Requirements:** Node.js 18+

**Setup:**
```bash
cd web/wallet-frontend
npm install
npm run dev
```

Frontend will run on 👉 http://localhost:5173

---

## 🔐 Mock Authentication

> 🧩 **Note:** Authentication is **mocked for demo purposes** — there is no password or real identity provider.

You can log in using the demo user:

| Field | Value |
|--------|--------|
| **User ID (GUID)** | `AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE` |
| **Password** | _(not required)_ |

The system will issue a **fake JWT token** that enables API access until logout.  
This approach simplifies the demo — in a production system, you would replace it with real authentication (e.g., IdentityServer, Auth0, etc.).

---

## 💡 Core Features

| Feature | Description |
|----------|--------------|
| **Login** | Mock JWT-based login (no password required) |
| **Wallet Balance** | Displays current wallet balance for the user |
| **Earn Points** | Choose a service and add “earn” points |
| **Burn Points** | Deduct points (spending or redemption) |
| **Transactions** | View full transaction history with pagination |
| **Services Management** | Fetch available services from backend |
| **Configuration Rules (Seeded)** | Each service has a base earning rule |

---

## 🧠 Demo Flow

1. Open the app → you’ll be redirected to **Login Page**.
2. Paste the mock User ID:
   ```
   AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE
   ```
   Then click **Login**.
3. The app reloads → navigates to **Wallet Page**.
4. Balance is fetched automatically.
5. Choose a **Service** → enter an amount → click **Earn**.
6. Wallet balance updates instantly.
7. Go to **Transactions Page** to see full log of wallet actions.

---

## 🧱 Backend API Overview

| Method | Endpoint | Description |
|---------|-----------|-------------|
| `POST` | `/api/auth/login` | Mock login — returns fake JWT + userId |
| `GET` | `/api/services` | Fetch all available services |
| `GET` | `/api/wallets/balance` | Get wallet balance for current user |
| `POST` | `/api/wallets/earn` | Add points (earn) |
| `POST` | `/api/wallets/burn` | Remove points (burn) |
| `GET` | `/api/wallets/transactions` | List wallet transactions |

Example `POST /api/wallets/earn` body:
```json
{
  "serviceId": "GUID-OF-SERVICE",
  "amount": 100,
  "externalReference": "EXT-001",
  "description": "Demo earning"
}
```

---

## 🧾 Seeded Data (for Demo)

| Entity | Example Data |
|--------|---------------|
| **User** | `AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE` |
| **Wallet** | Balance: `1000` |
| **Services** | 3 example services: Food Delivery, E-commerce, Ride Sharing |
| **Rules** | Default rules for each service (earn 1 point per base amount) |

---

## 🧩 Future Improvements (Bonus Ideas)

- ✅ Add `/api/services/{id}/rules` endpoint to view configuration rules
- ✅ Add Admin dashboard to manage rules dynamically
- 🔒 Replace mock JWT with real authentication provider
- 📈 Add expiry & multipliers for special promotions
- 🧮 Add background job to recalculate points after rule updates

---

## 👨‍💻 Developer Notes

This repo focuses on **architecture & workflow demonstration** rather than production security.  
The **mock JWT** lets us focus on wallet logic without external auth dependencies.

If you’re reviewing this demo:
- Check the **WalletService** implementation — it handles **idempotency**, **optimistic concurrency**, and **rule-based earning**.
- Review `ServiceRepository` + `ConfigurationRuleRepository` — clean data access abstraction.
- Examine `WalletPage.tsx` — dynamic service selection and API integration.

---

## 🏁 License
This demo project is provided for educational and portfolio purposes only.  
Feel free to fork or adapt it for your own learning.

---

**© 2025 Wallet Demo — built with ❤️ using .NET 8 + React (TS + MUI)**
