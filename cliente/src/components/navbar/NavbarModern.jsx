import { motion } from "framer-motion";
import { FaFilm, FaTv, FaSignOutAlt } from "react-icons/fa";

const tabs = [
  { name: "Películas", key: "peliculas", icon: <FaFilm /> },
  { name: "Series", key: "series", icon: <FaTv /> }
];

function NavbarModern({ seccion, setSeccion, onLogout }) {
  return (
    <nav className="w-full flex justify-center py-3 px-4 sticky top-0 z-50 bg-gradient-to-r from-gray-900 via-black to-gray-900 backdrop-blur-md shadow-md">
      <div className="flex items-center justify-between w-full max-w-6xl">
        {/* Logo */}
        <div className="flex items-center gap-2 text-white text-2xl font-bold">
        </div>
        {/* Tabs */}
        <div className="flex gap-1 relative bg-gray-800 rounded-full p-1 shadow-inner">
          {tabs.map((tab) => (
            <button
              key={tab.key}
              onClick={() => setSeccion(tab.key)}
              className={`relative flex items-center gap-2 px-5 py-2 rounded-full font-medium text-sm transition-all
                ${seccion === tab.key
                  ? "text-white"
                  : "text-gray-400 hover:text-white"
                }`}
              style={{ zIndex: 2 }}
            >
              <span className="text-lg">{tab.icon}</span>
              {tab.name}
              {seccion === tab.key && (
                <motion.div
                  layoutId="pill-nav"
                  className="absolute inset-0 bg-purple-600 bg-opacity-80 rounded-full"
                  style={{ zIndex: -1 }}
                  transition={{ type: "spring", stiffness: 500, damping: 30 }}
                />
              )}
            </button>
          ))}
        </div>

        {/* Salida */}
        <button
          onClick={onLogout}
          className="flex items-center gap-2 text-sm font-semibold text-gray-300 hover:text-red-400 transition"
        >
          <FaSignOutAlt className="text-lg" />
          Salir
        </button>
      </div>
    </nav>
  );
}

export default NavbarModern;
