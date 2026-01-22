# FoodHub Delivery Management System

A small, runnable WinForms solution that connects to the existing **FoodDeliveryDB** SQL Server database and supports customer registration, order creation, rider assignment, and order status tracking.

## Solution Structure
- **FoodHub.sln**
- **FoodHub.Models** (POCO models)
- **FoodHub.Data** (ADO.NET repositories + DB helper)
- **FoodHub.Services** (business logic + validation)
- **FoodHub.UI** (WinForms UI)

## How to Configure the Connection String
Update `FoodHub.UI/App.config`:

```xml
<add key="FoodHubDb" value="Server=(localdb)\MSSQLLocalDB;Database=FoodDeliveryDB;Trusted_Connection=True;TrustServerCertificate=True;" />
```

If using SQL Express or another instance, replace the server value accordingly.

## How to Run
1. Open `FoodHub.sln` in Visual Studio 2022+.
2. Ensure the SQL Server database **FoodDeliveryDB** already exists with the required schema.
3. Set **FoodHub.UI** as the startup project.
4. Run the solution.

## Screens & Workflows
### 1) Customer Registration
- Add new customers with validation (required fields, unique NIC).
- Optional CRUD via DataGridView (update/delete after selecting a row).

### 2) New Order
- Select a customer.
- Choose items from the food list and add quantities to the cart.
- Select payment method and place order (Orders + OrderItem inserted in a transaction).

### 3) Rider Assignment
- Pick a pending/unassigned order.
- Choose a rider and motorbike.
- Assign with start/end meter validation.

### 4) Order Status
- Search by Order ID or customer name.
- Displays order details and line items.

## Example Test Data Steps
1. Add a customer in **Customer Registration**.
2. Ensure FoodItem, Rider, and Motorbike tables have data in SSMS.
3. Place an order in **New Order**.
4. Assign a rider in **Rider Assignment**.
5. Search the order in **Order Status**.

## Key SQL Queries (ADO.NET)
### Insert Customer
```sql
INSERT INTO Customer (Name, NIC, DOB, ContactNo, LocationNo, Lane, Street, City)
VALUES (@Name, @NIC, @DOB, @ContactNo, @LocationNo, @Lane, @Street, @City);
```

### Get Food Items
```sql
SELECT FoodItemID, ItemName, Price
FROM FoodItem
ORDER BY ItemName;
```

### Create Order + Items (transaction)
```sql
INSERT INTO Orders (CustomerID, RiderID, OrderDate, OrderTime, Status, PaymentMethod, DispatchTime, OrderAmount)
VALUES (@CustomerID, @RiderID, @OrderDate, @OrderTime, @Status, @PaymentMethod, @DispatchTime, @OrderAmount);

INSERT INTO OrderItem (OrderID, ItemID, Quantity)
VALUES (@OrderID, @ItemID, @Quantity);
```

### Pending Orders for Assignment
```sql
SELECT OrderID
FROM Orders
WHERE RiderID IS NULL OR Status = 'Pending'
ORDER BY OrderDate DESC, OrderTime DESC;
```

### Assign Rider + Update Order
```sql
INSERT INTO RiderBikeAssignment (RiderID, BikeRegNo, AssignmentDate, StartMeter, EndMeter)
VALUES (@RiderID, @BikeRegNo, @AssignmentDate, @StartMeter, @EndMeter);

UPDATE Orders
SET RiderID = @RiderID,
    Status = 'Dispatched',
    DispatchTime = @DispatchTime
WHERE OrderID = @OrderID;
```

### Order Status Search (joins)
```sql
SELECT o.OrderID,
       c.Name AS CustomerName,
       COALESCE(r.FirstName + ' ' + r.LastName, '') AS RiderName,
       o.OrderDate,
       o.OrderAmount,
       o.Status,
       o.PaymentMethod,
       o.DispatchTime
FROM Orders o
INNER JOIN Customer c ON o.CustomerID = c.CustomerID
LEFT JOIN Rider r ON o.RiderID = r.RiderID
WHERE (@OrderId IS NOT NULL AND o.OrderID = @OrderId)
   OR (@OrderId IS NULL AND c.Name LIKE @CustomerName)
ORDER BY o.OrderDate DESC, o.OrderTime DESC;
```

### Order Line Items
```sql
SELECT f.ItemName,
       f.Price,
       oi.Quantity
FROM OrderItem oi
INNER JOIN FoodItem f ON oi.ItemID = f.FoodItemID
WHERE oi.OrderID = @OrderID;
```

## Notes
- This solution assumes the SQL Server schema already exists in **FoodDeliveryDB** exactly as provided.
- All queries are parameterized to prevent SQL injection.
- Duplicate NIC inserts are handled with friendly messages.
