import { useEffect, useContext } from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

import Store from "./pages/Store";
import Cart from "./pages/Cart";
import Library from "./pages/Library";
import AppDetails from "./pages/AppDetails";
import Login from "./pages/Login";
import Register from "./pages/Register";
import About from "./pages/About";
import SearchResults from "./pages/SearchResults";
import ViewWallet from "./pages/ViewWallet";
import GoogleAuthRedirect from "./pages/GoogleAuthRedirect";
import AccountDetails from "./pages/AccountDetails";
import PurchaseHistory from "./pages/PurchaseHistory";
import ChangeEmail from "./pages/ChangeEmail";
import UserProfile from "./pages/UserProfile";
import EditProfile from "./pages/EditProfile";
import Community from "./pages/Community";
import CreatePost from "./pages/CreatePost";
import AdminPanel from "./pages/AdminPanel";
import PrivacyPolicy from "./pages/PrivacyPolicy";
import TermsOfService from "./pages/TermsOfService";
import RefundPolicy from "./pages/RefundPolicy";
import Support from "./pages/Support";
import VerifyEmail from "./pages/VerifyEmail";
import DeleteAccount from "./pages/DeleteAccount";
import ChangeUsername from "./pages/ChangeUsername";
import ChangePassword from "./pages/ChangePassword";
import ForgotPassword from "./pages/ForgotPassword";
import ResetPassword from "./pages/ResetPassword";
import ConfirmEmailChange from "./pages/ConfirmEmailChange";
import Wishlist from "./pages/Wishlist";

import Navbar from "./components/Navbar";
import Footer from "./components/Footer";
import ProtectedRoute from "./components/ProtectedRoute";

import { useCart } from "./context/CartContext";
import AuthContext from "./context/AuthContext";

import cartService from "./services/cartService";

function App() {
  const { user, isAuthenticated } = useContext(AuthContext);
  const { setCart } = useCart();

  useEffect(() => {
    const syncCart = async () => {
      if (isAuthenticated && user?.userId) {
        const dbCart = await cartService.getCart(user.userId);
        setCart(dbCart);
      } else {
        setCart([]); 
      }
    };

    syncCart();
  }, [isAuthenticated, user?.userId]);

  return (
    <Router>
      <Navbar />
      <Routes>
        <Route path="/google-auth" element={<GoogleAuthRedirect />} />
        <Route path="/" element={<Store />} />
        <Route path="/about" element={<About />} />
        <Route path="/cart" element={<Cart />} />
        <Route path="/appid/:id" element={<AppDetails />} />
        <Route path="/login" element={<Login />} />
        <Route path="/search" element={<SearchResults />} />
        <Route path="/register" element={<Register />} />
        <Route path="/profile/:username" element={<UserProfile />} />
        <Route path="/community" element={<Community />} />
        <Route path="/privacy-policy" element={<PrivacyPolicy />} />
        <Route path="/terms-of-service" element={<TermsOfService />} />
        <Route path="/refund-policy" element={<RefundPolicy />} />
        <Route path="/support" element={<Support />} />
        <Route path="/verify-email" element={<VerifyEmail />} />
        <Route path="/delete-account" element={<DeleteAccount />} />
        <Route path="/forgot-password" element={<ForgotPassword />} />
        <Route path="/reset-password/:token" element={<ResetPassword />} />
        <Route path="/confirm-email-change" element={<ConfirmEmailChange />} />
        <Route
          path="/account"
          element={
            <ProtectedRoute>
              <AccountDetails />
            </ProtectedRoute>
          }
        />
        <Route
          path="/create-post"
          element={
            <ProtectedRoute>
              <CreatePost />
            </ProtectedRoute>
          }
        />
        <Route
          path="/admin"
          element={
            <ProtectedRoute allowedRoles={["Admin"]}>
              <AdminPanel />
            </ProtectedRoute>
          }
        />
        <Route
          path="/wishlist"
          element={
            <ProtectedRoute>
              <Wishlist />
            </ProtectedRoute>
          }
        />
        <Route
          path="/account/edit"
          element={
            <ProtectedRoute>
              <EditProfile />
            </ProtectedRoute>
          }
        />
        <Route
          path="/purchase-history"
          element={
            <ProtectedRoute>
              <PurchaseHistory />
            </ProtectedRoute>
          }
        />
        <Route
          path="/change-email"
          element={
            <ProtectedRoute>
              <ChangeEmail />
            </ProtectedRoute>
          }
        />
        <Route
          path="/change-username"
          element={
            <ProtectedRoute>
              <ChangeUsername />
            </ProtectedRoute>
          }
        />
        <Route
          path="/change-password"
          element={
            <ProtectedRoute>
              <ChangePassword />
            </ProtectedRoute>
          }
        />
        <Route
          path="/library"
          element={
            <ProtectedRoute>
              <Library />
            </ProtectedRoute>
          }
        />
        <Route
          path="/wallet"
          element={
            <ProtectedRoute>
              <ViewWallet />
            </ProtectedRoute>
          }
        />
      </Routes>
      <Footer />
    </Router>
  );
}

export default App;
