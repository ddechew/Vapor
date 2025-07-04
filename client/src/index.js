import ReactDOM from 'react-dom/client';
import './styles/index.css';
import App from './App';
import reportWebVitals from './reportWebVitals';

import { CartProvider } from './context/CartContext';
import { AuthProvider } from './context/AuthContext';
import { WishlistProvider } from './context/WishlistContext';
import { NotificationProvider } from './context/NotificationContext'

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
    <CartProvider>
      <AuthProvider>
        <NotificationProvider>
          <WishlistProvider>
            <App />
          </WishlistProvider>
        </NotificationProvider>
      </AuthProvider>
    </CartProvider>
);

reportWebVitals();
