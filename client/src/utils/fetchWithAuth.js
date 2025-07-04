import authService from "../services/authService";

const AUTH_API_BASE = "https://localhost:7003/api/auth";

export default async function fetchWithAuth(url, options = {}, retry = true) {
  options = options || {};
  options.headers = options.headers || {};

  const token = localStorage.getItem("token");
  if (token) options.headers.Authorization = `Bearer ${token}`;

  let response = await fetch(url, options);

  if (response.status === 401 && retry && !url.includes("/logout")) {
    const refreshToken = localStorage.getItem("refreshToken");
    const refreshResponse = await fetch(
      `${AUTH_API_BASE}/refresh-token`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ refreshToken }),
      }
    );

    if (!refreshResponse.ok) {
      await authService.logout();
      localStorage.removeItem("token");
      localStorage.removeItem("refreshToken");
      window.location.href = "/login";
      return response;
    }

    const data = await refreshResponse.json();
    localStorage.setItem("token", data.token);
    localStorage.setItem("refreshToken", data.refreshToken);

    options.headers.Authorization = `Bearer ${data.token}`;
    response = await fetch(url, options);
  }

  return response;
}