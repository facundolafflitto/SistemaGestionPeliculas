import React, { useEffect, useState } from "react";
import PeliculaCard from "../components/cards/PeliculaCard";
import FormularioPelicula from "../components/forms/FormularioPelicula";
import DetalleModalSerie from "../components/modals/DetalleModalSerie";
import { motion } from "framer-motion";

const API_URL = import.meta.env.VITE_API_URL;

const Series = ({ onLogout }) => {
  const [series, setSeries] = useState([]);
  const [cargando, setCargando] = useState(true);
  const [busqueda, setBusqueda] = useState("");
  const [modalAbierto, setModalAbierto] = useState(false);
  const [esEdicion, setEsEdicion] = useState(false);
  const [serie, setSerie] = useState({
    titulo: "",
    genero: "",
    año: "",
    imagenUrl: ""
  });
  const [serieDetalle, setSerieDetalle] = useState(null);
  const [erroresFormulario, setErroresFormulario] = useState("");
  const [filtroGenero, setFiltroGenero] = useState("");

  const [favoritas, setFavoritas] = useState([]);
  const userId = localStorage.getItem("userId");

  const token = localStorage.getItem("token");
  const rol = localStorage.getItem("rol") || "";
  const esAdmin = rol === "admin";

  useEffect(() => {
    fetchSeries();
    fetchFavoritas();
    // el tema lo maneja la navbar globalmente
  }, []);

  const fetchSeries = () => {
    fetch(`${API_URL}/api/series`, {
      headers: { Authorization: `Bearer ${token}` }
    })
      .then((res) => {
        if (!res.ok) throw new Error("No autorizado");
        return res.json();
      })
      .then((data) => setSeries(data))
      .catch((err) => {
        console.error(err);
        if (err.message === "No autorizado") onLogout();
      })
      .finally(() => setCargando(false));
  };

  const fetchFavoritas = () => {
    if (!userId) return;
    fetch(`${API_URL}/api/usuarios/${userId}/seriesfavoritas`, {
      headers: { Authorization: `Bearer ${token}` }
    })
      .then(res => res.json())
      .then(setFavoritas)
      .catch(console.error);
  };

  const toggleFavorita = (serieId) => {
    if (!userId) return;
    const esFavorita = favoritas.some(s => s.id === serieId);
    const method = esFavorita ? "DELETE" : "POST";
    fetch(`${API_URL}/api/usuarios/${userId}/seriesfavoritas/${serieId}`, {
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

  const eliminarSerie = (id) => {
    if (!window.confirm("¿Estás seguro que deseas eliminar esta serie?")) return;
    fetch(`${API_URL}/api/series/${id}`, {
      method: "DELETE",
      headers: { Authorization: `Bearer ${token}` }
    })
      .then((res) => {
        if (!res.ok) throw new Error("No se pudo eliminar la serie");
        fetchSeries();
        fetchFavoritas();
      })
      .catch((err) => {
        console.error(err);
        alert("Error al eliminar. Es posible que la serie no exista o no estés autorizado.");
      });
  };

  const iniciarEdicion = (s) => {
    setSerie(s);
    setEsEdicion(true);
    setModalAbierto(true);
  };

  const abrirModalNueva = () => {
    setSerie({ titulo: "", genero: "", año: "", imagenUrl: "" });
    setEsEdicion(false);
    setModalAbierto(true);
  };

  const cancelarEdicion = () => {
    setSerie({ titulo: "", genero: "", año: "", imagenUrl: "" });
    setModalAbierto(false);
    setEsEdicion(false);
    setErroresFormulario("");
  };

  const guardarCambios = () => {
    const url = serie.id
      ? `${API_URL}/api/series/${serie.id}`
      : `${API_URL}/api/series`;
    const method = serie.id ? "PUT" : "POST";

    fetch(url, {
      method,
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${token}`
      },
      body: JSON.stringify({ ...serie, año: parseInt(serie.año) })
    })
      .then(async (res) => {
        if (!res.ok) {
          const contentType = res.headers.get("content-type");
          if (res.status === 401) throw new Error("No autorizado");

          if (res.status === 400 && contentType?.includes("application/json")) {
            const errorJson = await res.json();
            const errores = errorJson.errors
              ? Object.values(errorJson.errors).flat()
              : [errorJson.title || "Error de validación"];
            throw new Error(errores.join("\n"));
          }

          const text = await res.text();
          throw new Error(text || "Error al guardar");
        }

        return res.json();
      })
      .then(() => {
        fetchSeries();
        fetchFavoritas();
        cancelarEdicion();
      })
      .catch((err) => {
        console.error(err);
        if (err.message === "No autorizado") onLogout();
        else setErroresFormulario(err.message);
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
      series.flatMap(s =>
        s.genero
          ? s.genero.split(",").map(g => mapaGeneros[g.trim()] || g.trim())
          : []
      )
    )
  ].filter(g => g);

  const seriesFiltradas = series.filter((s) => {
    const generosDeSerie = s.genero
      ? s.genero.split(",").map(g => mapaGeneros[g.trim()] || g.trim())
      : [];
    const generoMatch = filtroGenero === "" || generosDeSerie.includes(filtroGenero);

    return (
      s.titulo.toLowerCase().includes(busqueda.toLowerCase()) && generoMatch
    );
  });

  return (
    <div className="p-6 bg-gray-100 dark:bg-gray-900 min-h-screen text-gray-900 dark:text-gray-100 transition-colors">
      {/* Header */}
      <div className="flex flex-col md:flex-row md:items-center md:justify-between mb-8 gap-4">
        <h2 className="text-3xl font-bold flex items-center gap-2">
          <span role="img" aria-label="tv">📺</span>
          Lista de Series
        </h2>
        <div className="flex gap-2 flex-wrap">
          <input
            type="text"
            placeholder="Buscar por título..."
            value={busqueda}
            onChange={(e) => setBusqueda(e.target.value)}
            className="p-2 border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 rounded text-gray-800 dark:text-gray-100 w-64"
          />
          <select
            className="p-2 border border-gray-300 dark:border-gray-600 bg-white dark:bg-gray-800 rounded text-gray-800 dark:text-gray-100"
            value={filtroGenero}
            onChange={(e) => setFiltroGenero(e.target.value)}
          >
            <option value="">Todos los géneros</option>
            {generosUnicos.map((g) => (
              <option key={g} value={g}>
                {g}
              </option>
            ))}
          </select>
          {esAdmin && (
            <button
              onClick={abrirModalNueva}
              className="bg-purple-600 text-white px-4 py-2 rounded shadow hover:bg-purple-700 transition"
            >
              + Nueva Serie
            </button>
          )}
          {/* 👇 sin botón de modo oscuro; lo maneja la navbar */}
        </div>
      </div>

      {/* Contenido principal */}
      {cargando ? (
        <p className="text-center text-gray-500">Cargando...</p>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
          <AnimatePresence>
            {seriesFiltradas.map((s, i) => (
              <motion.div
                key={s.id}
                initial={{ opacity: 0, y: 30 }}
                animate={{ opacity: 1, y: 0 }}
                exit={{ opacity: 0, y: 30 }}
                transition={{ duration: 0.38, delay: i * 0.06 }}
                whileHover={{ scale: 1.04, boxShadow: "0 8px 32px rgba(80,80,160,0.18)" }}
                className="relative isolate overflow-visible rounded shadow bg-gray-800 dark:bg-gray-800 cursor-pointer"
              >
                <PeliculaCard
                  pelicula={s}
                  onEdit={iniciarEdicion}
                  onDelete={eliminarSerie}
                  esAdmin={esAdmin}
                  esFavorita={favoritas.some(f => f.id === s.id)}
                  onToggleFavorita={() => toggleFavorita(s.id)}
                  onVerDetalle={() => setSerieDetalle(s)}
                />
              </motion.div>
            ))}
          </AnimatePresence>
        </div>
      )}

      {/* Modal de detalle */}
      {serieDetalle && (
        <DetalleModalSerie serie={serieDetalle} onClose={() => setSerieDetalle(null)} />
      )}

      {/* Modal de formulario */}
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
                className="absolute top-2 right-2 text-xl text-gray-500 hover:text-red-600"
                onClick={cancelarEdicion}
                title="Cerrar"
              >
                ×
              </button>
              {erroresFormulario && (
                <div className="mb-4 text-red-500 text-sm whitespace-pre-line">
                  {erroresFormulario}
                </div>
              )}
              <FormularioPelicula
                pelicula={serie}
                setPelicula={setSerie}
                guardarCambios={guardarCambios}
                cancelarEdicion={cancelarEdicion}
                esEdicion={esEdicion}
              />
            </motion.div>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  );
};

export default Series;
