import React, { useEffect, useState } from "react";
import PeliculaCard from "../components/cards/PeliculaCard";
import FormularioPelicula from "../components/forms/FormularioPelicula";
import DetalleModal from "../components/modals/DetalleModal";
import { motion } from "framer-motion";

const Peliculas = ({ onLogout }) => {
  const [peliculas, setPeliculas] = useState([]);
  const [cargando, setCargando] = useState(true);
  const [busqueda, setBusqueda] = useState("");
  const [modalAbierto, setModalAbierto] = useState(false);
  const [esEdicion, setEsEdicion] = useState(false);
  const [pelicula, setPelicula] = useState({
    titulo: "",
    genero: "",
    año: "",
    imagenUrl: ""
  });
  const [peliculaDetalle, setPeliculaDetalle] = useState(null);
  const [erroresFormulario, setErroresFormulario] = useState("");
  const [filtroGenero, setFiltroGenero] = useState("");

  // ---- FAVORITAS ----
  const [favoritas, setFavoritas] = useState([]);
  const userId = localStorage.getItem("userId");
  // -------------------

  const apiUrl = import.meta.env.VITE_API_URL;
  const token = localStorage.getItem("token");
  const rol = localStorage.getItem("rol") || "";
  const esAdmin = rol === "admin";

  const fetchPeliculas = () => {
    fetch(`${apiUrl}/api/peliculas`, {
      headers: { Authorization: `Bearer ${token}` }
    })
      .then(res => {
        if (!res.ok) throw new Error("No autorizado");
        return res.json();
      })
      .then(data => setPeliculas(data))
      .catch(err => {
        console.error(err);
        if (err.message === "No autorizado") onLogout();
      })
      .finally(() => setCargando(false));
  };

  // ---- TRAER FAVORITAS DEL USUARIO ----
  const fetchFavoritas = () => {
    if (!userId) return;
    fetch(`${apiUrl}/api/usuarios/${userId}/favoritas`, {
      headers: { Authorization: `Bearer ${token}` }
    })
      .then(res => res.json())
      .then(setFavoritas)
      .catch(console.error);
  };

  useEffect(() => {
    fetchPeliculas();
    fetchFavoritas();
    // el modo oscuro ahora lo maneja la navbar globalmente
  }, []);
  // --------------------------------------

  // AGREGAR O QUITAR FAVORITA
  const toggleFavorita = (peliculaId) => {
    if (!userId) return;
    const esFavorita = favoritas.some(p => p.id === peliculaId);
    const method = esFavorita ? "DELETE" : "POST";
    fetch(`${apiUrl}/api/usuarios/${userId}/favoritas/${peliculaId}`, {
      method,
      headers: { Authorization: `Bearer ${token}` }
    })
      .then(res => {
        if (!res.ok) throw new Error("Error en favoritas");
        return res.json();
      })
      .then(setFavoritas)
      .catch(console.error);
  };

  const eliminarPelicula = (id) => {
    if (!window.confirm("¿Eliminar esta película?")) return;
    fetch(`${apiUrl}/api/peliculas/${id}`, {
      method: "DELETE",
      headers: { Authorization: `Bearer ${token}` }
    })
      .then(res => {
        if (!res.ok) throw new Error("No se pudo eliminar");
        fetchPeliculas();
        fetchFavoritas(); // refresca favoritas si eliminan una
      })
      .catch(err => {
        console.error(err);
        alert("Error al eliminar.");
      });
  };

  const iniciarEdicion = (p) => {
    setPelicula(p);
    setEsEdicion(true);
    setModalAbierto(true);
  };

  const abrirModalNueva = () => {
    setPelicula({ titulo: "", genero: "", año: "", imagenUrl: "" });
    setEsEdicion(false);
    setModalAbierto(true);
  };

  const cancelarEdicion = () => {
    setPelicula({ titulo: "", genero: "", año: "", imagenUrl: "" });
    setModalAbierto(false);
    setEsEdicion(false);
  };

  const guardarCambios = () => {
    const url = pelicula.id
      ? `${apiUrl}/api/peliculas/${pelicula.id}`
      : `${apiUrl}/api/peliculas`;
    const method = pelicula.id ? "PUT" : "POST";

    fetch(url, {
      method,
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify({ ...pelicula, año: parseInt(pelicula.año) })
    })
      .then(async res => {
        if (!res.ok) {
          const contentType = res.headers.get("content-type");
          if (res.status === 401) throw new Error("No autorizado");

          if (res.status === 400 && contentType?.includes("application/json")) {
            const errorJson = await res.json();
            const errores = errorJson.errors
              ? Object.values(errorJson.errors).flat().join("\n")
              : errorJson.title || "Error de validación";
            setErroresFormulario(errores);
            throw new Error(errores);
          }

          const text = await res.text();
          setErroresFormulario(text);
          throw new Error(text || "Error al guardar");
        }

        return res.json();
      })
      .then(() => {
        fetchPeliculas();
        fetchFavoritas(); // refresca favoritas si cambia algo
        cancelarEdicion();
        setErroresFormulario("");
      })
      .catch(err => {
        console.error(err);
        if (err.message === "No autorizado") onLogout();
      });
  };

  const mapaGeneros = {
    Action: "Acción",
    Drama: "Drama",
    Comedy: "Comedia",
    Thriller: "Suspenso",
    Horror: "Terror",
    Romance: "Romance",
    Adventure: "Aventura",
    "Sci-Fi": "Ciencia ficción",
    Fantasy: "Fantasía",
    Documentary: "Documental",
    Crime: "Crimen",
    Mystery: "Misterio",
    Animation: "Animación"
  };

  const generosUnicos = [
    ...new Set(
      peliculas.flatMap(p =>
        p.genero ? p.genero.split(",").map(g => mapaGeneros[g.trim()] || g.trim()) : []
      )
    )
  ];

  const peliculasFiltradas = peliculas.filter(p => {
    const generos = p.genero
      ? p.genero.split(",").map(g => mapaGeneros[g.trim()] || g.trim())
      : [];
    return (
      p.titulo.toLowerCase().includes(busqueda.toLowerCase()) &&
      (filtroGenero === "" || generos.includes(filtroGenero))
    );
  });

  return (
    <div className="p-6 bg-gray-100 dark:bg-gray-900 min-h-screen text-gray-900 dark:text-gray-100 transition-colors">
      <div className="flex flex-col md:flex-row md:items-center md:justify-between mb-8 gap-4">
        <h2 className="text-3xl font-bold flex items-center gap-2">🎬 Lista de Películas</h2>
        <div className="flex gap-2 flex-wrap">
          <input
            type="text"
            placeholder="Buscar..."
            value={busqueda}
            onChange={(e) => setBusqueda(e.target.value)}
            className="p-2 border rounded bg-white dark:bg-gray-800"
          />
          <select
            value={filtroGenero}
            onChange={(e) => setFiltroGenero(e.target.value)}
            className="p-2 border rounded bg-white dark:bg-gray-800"
          >
            <option value="">Todos los géneros</option>
            {generosUnicos.map(g => <option key={g}>{g}</option>)}
          </select>
          {esAdmin && (
            <button onClick={abrirModalNueva} className="bg-purple-600 text-white px-4 py-2 rounded">
              + Nueva Película
            </button>
          )}
          {/* 👇 sin botón de modo oscuro; lo maneja la navbar */}
        </div>
      </div>

      {cargando ? (
        <p className="text-center">Cargando...</p>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6 overflow-visible">
          <AnimatePresence>
            {peliculasFiltradas.map((p, i) => (
              <motion.div
                key={p.id}
                initial={{ opacity: 0, y: 30 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, y: 30 }}
                transition={{ duration: 0.3, delay: i * 0.05 }}
                whileHover={{ scale: 1.05 }}
                className="relative isolate overflow-visible"
              >
                <PeliculaCard
                  pelicula={p}
                  onDelete={eliminarPelicula}
                  onEdit={iniciarEdicion}
                  esAdmin={esAdmin}
                  esFavorita={favoritas.some(f => f.id === p.id)}
                  onToggleFavorita={() => toggleFavorita(p.id)}
                  onVerDetalle={() => setPeliculaDetalle(p)}
                />
              </motion.div>
            ))}
          </AnimatePresence>
        </div>
      )}

      {peliculaDetalle && (
        <DetalleModal pelicula={peliculaDetalle} onClose={() => setPeliculaDetalle(null)} />
      )}

      <AnimatePresence>
        {modalAbierto && (
          <motion.div
            className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-40 z-50"
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
          >
            <motion.div
              initial={{ scale: 0.92, opacity: 0 }}
              animate={{ scale: 1, opacity: 1 }}
              exit={{ scale: 0.95, opacity: 0 }}
              transition={{ duration: 0.22 }}
              className="bg-white dark:bg-gray-800 p-8 rounded shadow-lg max-w-md w-full relative"
            >
              <button
                className="absolute top-2 right-2 text-xl"
                onClick={cancelarEdicion}
              >
                ×
              </button>
              <FormularioPelicula
                pelicula={pelicula}
                setPelicula={setPelicula}
                guardarCambios={guardarCambios}
                cancelarEdicion={cancelarEdicion}
                esEdicion={esEdicion}
                errores={erroresFormulario}
              />
            </motion.div>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  );
};

export default Peliculas;
