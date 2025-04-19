# Travel and Food Management System

## Project Overview
The **Travel Food CMS** is a comprehensive web application that manages travel destinations, restaurants, orders, and users to provide a seamless food ordering experience for travelers. Built with **ASP.NET Core**, the system demonstrates entity relationships using both a **Web API** for backend functionality and **MVC** for the user interface.

## Team Contributions

Sonia worked on implementing all the MVC, views models and API controllers along with authentication. 

Tianrui Zhu focused on the data models, view models, DTOs, database context, views, extra features, and overall system architecture.

## Controllers and Views
The application contains the following main controllers:

### **Destinations**
- `DestinationsPage` - MVC controller for web UI
- `Destinations` - API controller for data access

### **Restaurants**
- `RestaurantsPage` - MVC controller for web UI
- `Restaurants` - API controller for data access

### **Orders**
- `OrdersPage` - MVC controller for web UI
- `Orders` - API controller for data access

### **OrderItems**
- `OrderItemsPage` - MVC controller for web UI
- `OrderItems` - API controller for data access

### **Users**
- `UsersPage` - MVC controller for web UI
- `Users` - API controller for data access

---

## View Details
### **Destinations Controller**
#### Views:
- **Index.cshtml** - Displays a card-based grid of all destinations.
- **Details.cshtml** - Shows details of a specific destination, including associated restaurants.
- **Create.cshtml** - Form to add a new destination with image upload.
- **Delete.cshtml** - Delete confirmation page.
- **Edit.cshtml** - Form to update a destination's information and image.

### **Restaurants Controller**
#### Views:
- **Index.cshtml** - Displays a card-based grid of all restaurants.
- **Details.cshtml** - Shows details of a restaurant, including orders.
- **Create.cshtml** - Form to add a new restaurant with image upload.
- **Delete.cshtml** - Delete confirmation page.
- **Edit.cshtml** - Form to update a restaurant's information.

### **Orders Controller**
#### Views:
- **Index.cshtml** - Displays all orders.
- **Details.cshtml** - Shows details of an order, including order items.
- **Create.cshtml** - Form to create a new order with dynamic order items.
- **Delete.cshtml** - Delete confirmation page.
- **Edit.cshtml** - Form to update order details and modify order items.

### **OrderItems Controller**
#### Views:
- **Index.cshtml** - Displays all order items.
- **Details.cshtml** - Shows details of an order item.
- **Create.cshtml** - Form to add a new order item.
- **Delete.cshtml** - Delete confirmation page.
- **Edit.cshtml** - Form to update order item details.

### **Users Controller**
#### Views:
- **Index.cshtml** - Displays all users.
- **Details.cshtml** - Shows user details and order history.
- **Create.cshtml** - Form to add a new user.
- **Delete.cshtml** - Delete confirmation page.
- **Edit.cshtml** - Form to update user information.

---

## Entity Models
- `Destination.cs` - Represents travel destinations.
- `Restaurant.cs` - Represents restaurants.
- `Order.cs` - Represents customer orders.
- `OrderItem.cs` - Represents individual order items.
- `User.cs` - Represents system users (customers, admins).

## Data Transfer Objects (DTOs)
- `DestinationDTO.cs`
- `RestaurantDTO.cs`
- `OrderDTO.cs`
- `OrderItemDTO.cs`
- `UserDTO.cs`

## View Models
- `DestinationViewModel.cs`
- `RestaurantViewModel.cs`
- `OrderViewModel.cs`
- `OrderItemViewModel.cs`
- `UserViewModel.cs`

## Database Context
- `ApplicationDbContext.cs` - Configures the Entity Framework context.

---

## API Endpoints
### **Destinations API**
- `GET /api/Destinations`
- `GET /api/Destinations/{id}`
- `PUT /api/Destinations/{id}`
- `POST /api/Destinations`
- `DELETE /api/Destinations/{id}`
- `GET /api/Destinations/{id}/Restaurants`

### **Restaurants API**
- `GET /api/Restaurants`
- `GET /api/Restaurants/{id}`
- `PUT /api/Restaurants/{id}`
- `POST /api/Restaurants`
- `DELETE /api/Restaurants/{id}`
- `GET /api/Restaurants/{id}/Orders`
- `GET /api/Restaurants/ByDestination/{destinationId}`

### **Orders API**
- `GET /api/Orders`
- `GET /api/Orders/{id}`
- `PUT /api/Orders/{id}`
- `POST /api/Orders`
- `DELETE /api/Orders/{id}`
- `GET /api/Orders/{id}/OrderItems`
- `GET /api/Orders/ByUser/{userId}`
- `GET /api/Orders/ByRestaurant/{restaurantId}`

### **OrderItems API**
- `GET /api/OrderItems`
- `GET /api/OrderItems/{id}`
- `PUT /api/OrderItems/{id}`
- `POST /api/OrderItems`
- `DELETE /api/OrderItems/{id}`
- `GET /api/OrderItems/ByOrder/{orderId}`
- `POST /api/OrderItems/AddMultiple`

### **Users API**
- `GET /api/Users`
- `GET /api/Users/{id}`
- `PUT /api/Users/{id}`
- `POST /api/Users`
- `DELETE /api/Users/{id}`
- `GET /api/Users/{id}/Orders`
- `POST /api/Users/Authenticate`


## Extra Features

### Authentication

The Travel & Food CMS implements a role-based authentication system with two primary user roles:
- **Admin**: Full system access (create, edit, delete)
- **User**: Read-only access to system information

Key Authentication Features:
- User registration and login
- Restricted access to admin-only functions

### Image Upload

Image upload functionality is integrated across multiple models:
- Supports uploading images 
- Stores images in the `wwwroot/images` directory

Key Upload Features:
- Automatic filename generation
- File extension preservation

### Pagination and Search

List views implement advanced browsing capabilities:
- Pagination for all major list views (Destinations)
- Configurable page size
- Search functionality across multiple fields

Features:
- Page number tracking
- Previous/Next page navigation



