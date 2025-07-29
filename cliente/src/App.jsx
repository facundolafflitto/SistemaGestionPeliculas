import React, { useState } from "react";
import Peliculas from "./views/Peliculas";
import Series from "./views/Series";
import Login from "./views/login";
import Register from "./views/register";
import NavbarModern from "./components/navbar/NavbarModern";

const App = () => {
  const [autenticado, setAutenticado] = useState(!!localStorage.getItem("token"));
  const [mostrarRegistro, setMostrarRegistro] = useState(false);
  const [seccion, setSeccion] = useState("peliculas");

  const handleLoginSuccess = () => setAutenticado(true);
  const handleLogout = () => {
    localStorage.removeItem("token");
    setAutenticado(false);
  };

  if (!autenticado) {
    return mostrarRegistro ? (
      <>
        <Register onRegisterSuccess={() => setMostrarRegistro(false)} />
        <p className="text-center mt-2">
          ¿Ya tienes cuenta?{" "}
          <button
            className="text-blue-600 underline"
            onClick={() => setMostrarRegistro(false)}
          >
            Iniciar sesión
          </button>
        </p>
      </>
    ) : (
      <>
        <Login onLoginSuccess={handleLoginSuccess} />
        <p className="text-center mt-2">
          ¿No tienes cuenta?{" "}
          <button
            className="text-green-600 underline"
            onClick={() => setMostrarRegistro(true)}
          >
            Regístrate
          </button>
        </p>
      </>
    );
  }

  return (
    <div>
      <NavbarModern seccion={seccion} setSeccion={setSeccion} onLogout={handleLogout} />

      {seccion === "peliculas" ? (
        <Peliculas onLogout={handleLogout} />
      ) : (
        <Series onLogout={handleLogout} />
      )}
    </div>
  );
};

export default App;