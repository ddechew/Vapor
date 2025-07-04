import { useState } from "react";

import UsersTable from "../components/AdminPanel/UsersTable";
import AppsTable from "../components/AdminPanel/AppsTable";
import RolesTable from "../components/AdminPanel/RolesTable";
import DevelopersTable from "../components/AdminPanel/DevelopersTable";
import AppTypesTable from "../components/AdminPanel/AppTypesTable";
import PostsTable from "../components/AdminPanel/PostsTable"
import PostsCommentsTable from "../components/AdminPanel/PostCommentsTable"
import PostLikesTable from "../components/AdminPanel/PostLikesTable"
import AppReviewsTable from "../components/AdminPanel/AppReviewsTable";
import GenresTable from "../components/AdminPanel/GenresTable";
import PublishersTable from "../components/AdminPanel/PublishersTable";
import AppImagesTable from "../components/AdminPanel/AppImagesTable";
import AppVideosTable from "../components/AdminPanel/AppVideosTable";
import CartItemsTable from "../components/AdminPanel/CartItemsTable";
import WishlistTable from "../components/AdminPanel/WishlistTable";
import PurchaseHistoryTable from "../components/AdminPanel/PurchaseHistoryTable";
import NotificationsTable from "../components/AdminPanel/NotificationsTable";
import UserLibrariesTable from "../components/AdminPanel/AppLibraryTable";

import "../styles/AdminPanel.css";

const AdminPanel = () => {
  const [activeTab, setActiveTab] = useState("Users");

  const tabs = [
    { key: "Users", label: "Users" },
    { key: "Roles", label: "Roles" },
    { key: "Apps", label: "Applications" },
    { key: "AppTypes", label: "Application Types" },
    { key: "AppLibrary", label: "User Libraries" },
    { key: "Genres", label: "Genres" },
    { key: "Developers", label: "Developers" },
    { key: "Publishers", label: "Publishers" },
    { key: "Posts", label: "Posts" },
    { key: "PostComments", label: "Comments" },
    { key: "PostLikes", label: "Post Likes" },
    { key: "AppReviews", label: "App Reviews" },
    { key: "AppImages", label: "App Images" },
    { key: "AppVideos", label: "App Videos" },
    { key: "CartItems", label: "Cart Items" },
    { key: "Wishlist", label: "Wishlist" },
    { key: "PurchaseHistory", label: "Purchase History" },
    { key: "Notifications", label: "Notifications" },
  ];

  const renderTable = () => {
    switch (activeTab) {
      case "Users":
        return <UsersTable />;
      case "Roles":
        return <RolesTable />;
      case "Developers":
        return <DevelopersTable />
      case "AppTypes":
        return <AppTypesTable />
      case "Apps":
        return <AppsTable />
      case "Genres":
        return <GenresTable />
      case "Publishers":
        return <PublishersTable />
      case "Posts":
        return <PostsTable />
      case "PostComments":
        return <PostsCommentsTable />
      case "PostLikes":
        return <PostLikesTable />
      case "AppReviews":
        return <AppReviewsTable />
      case "AppImages":
        return <AppImagesTable />
      case "AppVideos":
        return <AppVideosTable />
      case "CartItems":
        return <CartItemsTable />
      case "Wishlist":
        return <WishlistTable />
      case "PurchaseHistory":
        return <PurchaseHistoryTable />
      case "Notifications":
        return <NotificationsTable />
      case "AppLibrary":
        return <UserLibrariesTable />  


      default:
        return <p>Select a table to manage.</p>;
    }
  };

  return (
    <div className="admin-panel">
      <h2>🛠️ Admin Dashboard</h2>

      <div className="admin-tabs">
        {tabs.map((tab) => (
          <button
            key={tab.key}
            className={activeTab === tab.key ? "active" : ""}
            onClick={() => setActiveTab(tab.key)}
          >
            {tab.label}
          </button>
        ))}
      </div>

      <div className="admin-content">
        {renderTable()}
        <div className="current-tab-info">
          <p>Current tab: {activeTab}</p>
        </div>
      </div>
    </div>
  );
};

export default AdminPanel;
