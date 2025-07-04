import { useEffect, useContext, useState } from "react";
import { useNavigate } from "react-router-dom";

import AuthContext from "../context/AuthContext";

const GoogleAuthRedirect = () => {
  const navigate = useNavigate();
  const { login, isAuthenticated } = useContext(AuthContext);
  const [tokenChecked, setTokenChecked] = useState(false);

  useEffect(() => {
    const urlParams = new URLSearchParams(window.location.search);
    const token = urlParams.get("token");
    const refreshToken = urlParams.get("refreshToken");

    if (token && refreshToken) {
      login({ token, refreshToken });
      setTokenChecked(true);
    } else {
      navigate("/login");
    }
  }, []);

  useEffect(() => {
    if (tokenChecked && isAuthenticated) {
      navigate("/"); 
    }
  }, [tokenChecked, isAuthenticated]);

  return <p>Logging you in...</p>;
};

export default GoogleAuthRedirect;
