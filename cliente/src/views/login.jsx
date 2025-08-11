import React, { useState } from "react";
import { jwtDecode } from "jwt-decode";

const Login = ({ onLoginSuccess }) => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const apiUrl = import.meta.env.VITE_API_URL;

  const handleLogin = (e) => {
    e.preventDefault();
    setError("");

    fetch(`${apiUrl}/api/auth/login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email, password }),
    })
      .then(async (res) => {
        if (!res.ok) {
          const msg = await res.text();
          throw new Error(msg || "Error al iniciar sesión");
        }
        return res.json(); // { token, userId, rol }
      })
      .then((data) => {
        // Guardar token
        localStorage.setItem("token", data.token);

        // Guardar rol (desde respuesta o desde el token como fallback)
        let rol = data.rol || "";
        try {
          const decoded = jwtDecode(data.token);
          rol =
            rol ||
            decoded["role"] ||
            decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
            "";
        } catch { /* ignore decode errors */ }
        localStorage.setItem("rol", rol);

        // Guardar userId (primero lo que manda el backend; si no, lo leo del token)
        let userId = data.userId;
        if (!userId) {
          try {
            const decoded = jwtDecode(data.token);
            userId =
              decoded.userId ||
              decoded.sub ||
              decoded["UserId"] ||
              decoded["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
          } catch { /* ignore decode errors */ }
        }

        if (!userId) {
          // Si por algún motivo no pudimos obtenerlo, avisamos y no seguimos
          throw new Error("No se pudo obtener el userId. Contactá al admin.");
        }

        localStorage.setItem("userId", String(userId));

        onLoginSuccess();
      })
      .catch((err) => {
        setError(err.message || "Error de autenticación");
      });
  };

  return (
    <div className="max-w-md mx-auto mt-20 bg-white dark:bg-gray-800 p-6 rounded shadow">
      <h2 className="text-2xl font-bold mb-4 text-center text-gray-800 dark:text-gray-100">
        Iniciar Sesión
      </h2>

      {error && (
        <p className="text-red-500 mb-3 text-center whitespace-pre-line">
          {error}
        </p>
      )}

      <form onSubmit={handleLogin}>
        <input
          type="email"
          placeholder="Email"
          className="w-full p-2 mb-3 border rounded dark:bg-gray-700 dark:text-white"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
          autoComplete="email"
        />

        <input
          type="password"
          placeholder="Contraseña"
          className="w-full p-2 mb-4 border rounded dark:bg-gray-700 dark:text-white"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
          autoComplete="current-password"
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
