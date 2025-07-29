import React, { useState } from "react";

const Register = ({ onRegisterSuccess }) => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const apiUrl = import.meta.env.VITE_API_URL;

  const handleRegister = (e) => {
    e.preventDefault();

    fetch(`${apiUrl}/api/auth/register`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ Email: email, Password: password }),
    })
      .then(async (res) => {
        if (!res.ok) {
          const msg = await res.text();
          throw new Error(msg || "Error al registrar");
        }
        return res.text();
      })
      .then(() => {
        setSuccess("¡Usuario registrado! Ahora puedes iniciar sesión.");
        setError("");
        if (onRegisterSuccess) onRegisterSuccess();
      })
      .catch((err) => {
        setError(err.message);
        setSuccess("");
      });
  };

  return (
    <div className="max-w-md mx-auto mt-20 bg-white dark:bg-gray-800 p-6 rounded shadow">
      <h2 className="text-2xl font-bold mb-4 text-center text-gray-800 dark:text-gray-100">
        Registro de Usuario
      </h2>
      {error && <p className="text-red-500 mb-3 text-center">{error}</p>}
      {success && <p className="text-green-500 mb-3 text-center">{success}</p>}
      <form onSubmit={handleRegister}>
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
          className="w-full bg-green-600 text-white py-2 rounded hover:bg-green-700 transition"
        >
          Registrarse
        </button>
      </form>
    </div>
  );
};

export default Register;
