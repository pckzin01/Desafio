import React, { createContext, useState, useEffect } from "react";

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [authToken, setAuthToken] = useState(null);
  const [user, setUser] = useState(null);

  useEffect(() => {
    const token = localStorage.getItem("authToken");
    if (token) {
      setAuthToken(token);
      const userData = parseJwt(token);
      setUser(userData);
    }
  }, []);

  const login = (token) => {
    setAuthToken(token);
    localStorage.setItem("authToken", token);

    const userData = parseJwt(token);
    setUser(userData);
  };

  const logout = () => {
    setAuthToken(null);
    setUser(null);
    localStorage.removeItem("authToken");
  };

  return (
    <AuthContext.Provider value={{ authToken, user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

function parseJwt(token) {
  try {
    const base64Url = token.split(".")[1];
    const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split("")
        .map((c) => `%${("00" + c.charCodeAt(0).toString(16)).slice(-2)}`)
        .join("")
    );
    return JSON.parse(jsonPayload);
  } catch (e) {
    console.error("Erro ao decodificar o token:", e);
    return null;
  }
}
