import React, { useState } from "react";
import { jwtDecode } from "jwt-decode"; 

const Login = ({ onLoginSuccess }) => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleLogin = (e) => {
    e.preventDefault();

    fetch("http://localhost:5183/api/auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email, password }),
    })
      .then(async (res) => {
        if (!res.ok) {
          const msg = await res.text();
          throw new Error(msg || "Error al iniciar sesión");
        }
        return res.json();
      })
      .then((data) => {
        localStorage.setItem("token", data.token);
        // Decodea el token y guarda el rol
        const decoded = jwtDecode(data.token);
        const rol =
          decoded["role"] ||
          decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
          "";
        localStorage.setItem("rol", rol);
        onLoginSuccess();
      })
      .catch((err) => {
        setError(err.message);
      });
  };

  return (
    <div className="max-w-md mx-auto mt-20 bg-white dark:bg-gray-800 p-6 rounded shadow">
      <h2 className="text-2xl font-bold mb-4 text-center text-gray-800 dark:text-gray-100">
        Iniciar Sesión
      </h2>
      {error && <p className="text-red-500 mb-3 text-center">{error}</p>}
      <form onSubmit={handleLogin}>
        <input
          type="email"
          placeholder="Email"
          className="w-full p-2 mb-3 border rounded dark:bg-gray-700 dark:text-white"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
        <input
          type="password"
          placeholder="Contraseña"
          className="w-full p-2 mb-4 border rounded dark:bg-gray-700 dark:text-white"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
        <button
          type="submit"
          className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700 transition"
        >
          Iniciar sesión
        </button>
      </form>
    </div>
  );
};

export default Login;
