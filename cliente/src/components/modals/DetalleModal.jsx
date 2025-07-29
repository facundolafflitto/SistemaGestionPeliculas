import React, { useEffect, useRef, useState } from "react";

const DetalleModal = ({ pelicula, onClose }) => {
  const fondoRef = useRef();
  const [trailerUrl, setTrailerUrl] = useState("");
  const [sinopsis, setSinopsis] = useState("");
  const [cargando, setCargando] = useState(true);
  const [tmdbRating, setTmdbRating] = useState("");
  const [tmdbVotes, setTmdbVotes] = useState("");

  // Cierra modal con ESC
  useEffect(() => {
    const handleKey = (e) => {
      if (e.key === "Escape") onClose();
    };
    window.addEventListener("keydown", handleKey);
    return () => window.removeEventListener("keydown", handleKey);
  }, [onClose]);

  // Cierra modal clickeando afuera
  const handleClickFondo = (e) => {
    if (e.target === fondoRef.current) onClose();
  };

  // Carga datos de TMDb
  useEffect(() => {
    if (!pelicula) return;
    setCargando(true);
    setTrailerUrl("");
    setSinopsis("");
    setTmdbRating("");
    setTmdbVotes("");
    fetch(
      `http://localhost:5183/api/peliculas/trailer-tmdb?titulo=${encodeURIComponent(
        pelicula.titulo
      )}`
    )
      .then((res) =>
        res.ok
          ? res.json()
          : { youtubeUrl: "", sinopsis: "Sin información.", tmdbRating: "", tmdbVotes: "" }
      )
      .then((data) => {
        setTrailerUrl(data.youtubeUrl || "");
        setSinopsis(data.sinopsis || "Sin información.");
        setTmdbRating(data.tmdbRating || "");
        setTmdbVotes(data.tmdbVotes || "");
      })
      .catch(() => {
        setTrailerUrl("");
        setSinopsis("Sin información.");
        setTmdbRating("");
        setTmdbVotes("");
      })
      .finally(() => setCargando(false));
  }, [pelicula]);

  if (!pelicula) return null;

  return (
    <div
      ref={fondoRef}
      className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-70 z-50"
      onClick={handleClickFondo}
    >
      <div
        className="bg-white dark:bg-gray-900 rounded shadow-lg max-w-lg w-full relative p-8"
        style={{ maxHeight: "90vh", overflowY: "auto" }}
      >
        <button
          onClick={onClose}
          className="absolute top-3 right-3 text-2xl text-gray-500 hover:text-red-600"
          title="Cerrar"
        >
          ×
        </button>
        <h2 className="text-2xl font-bold mb-2">{pelicula.titulo}</h2>
        <p className="text-gray-500 mb-1">{pelicula.año}</p>
        <p className="mb-2 text-sm">{pelicula.genero}</p>
        <img
          src={pelicula.imagenUrl}
          alt={pelicula.titulo}
          className="mb-4 rounded max-h-72 mx-auto"
        />

        <div className="mb-2">
          <span className="font-bold">Calificación mundial: </span>
          {tmdbRating
            ? (
              <>
                <span className="text-yellow-500 font-bold">{tmdbRating}</span>
                <span className="text-gray-400 text-sm ml-2">
                  ({tmdbVotes} votos)
                </span>
              </>
            )
            : <span className="text-gray-400">Sin calificación.</span>
          }
        </div>

        <div className="mb-2">
          <span className="font-bold">Sinopsis: </span>
          {cargando ? (
            <span className="italic text-gray-500">Buscando sinopsis...</span>
          ) : (
            <span>{sinopsis}</span>
          )}
        </div>
        <div className="mb-2">
          <span className="font-bold">Tráiler: </span>
          {cargando ? (
            <div className="my-3">Buscando tráiler...</div>
          ) : trailerUrl ? (
            <div className="my-3 aspect-video">
              <iframe
                width="100%"
                height="315"
                src={trailerUrl.replace("watch?v=", "embed/")}
                title="Tráiler"
                frameBorder="0"
                allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                allowFullScreen
              ></iframe>
            </div>
          ) : (
            <div className="my-3 text-red-600">No hay tráiler disponible.</div>
          )}
        </div>
      </div>
    </div>
  );
};

export default DetalleModal;
