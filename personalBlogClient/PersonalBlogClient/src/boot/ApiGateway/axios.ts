import axios from "axios";

const api = axios.create({
  baseURL: import.meta.env.QCLI_API_URL
});

// Request Interceptor: Attach JWT Bearer Token if available
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("pdq_auth_token");
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response Interceptor: Handle global 401 unauthorized
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem("pdq_auth_token");
      localStorage.removeItem("pdq_auth_user");
      // If we are currently on admin route, redirect to login
      if (window.location.hash.startsWith("#/admin") || window.location.pathname.startsWith("/admin")) {
        window.location.hash = "#/login";
      }
    }
    return Promise.reject(error);
  }
);

export default api;