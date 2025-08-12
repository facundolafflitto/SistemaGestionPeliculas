import { motion as Motion } from "framer-motion";
import { FaFilm, FaTv, FaSignOutAlt, FaChartPie, FaMoon, FaSun } from "react-icons/fa";
import useDarkMode from "../../hooks/useDarkMode";

const tabs = [
  { name: "Películas", key: "peliculas", icon: <FaFilm /> },
  { name: "Series",    key: "series",    icon: <FaTv /> },
  { name: "Dashboard", key: "dashboard", icon: <FaChartPie /> },
];

function NavbarModern({ seccion, setSeccion, onLogout }) {
  const { isDark, toggle } = useDarkMode();

  return (
    <nav className="w-full flex justify-center py-3 px-4 sticky top-0 z-50
                    bg-gradient-to-r from-gray-100 via-white to-gray-100 dark:from-gray-900 dark:via-black dark:to-gray-900
                    backdrop-blur-md shadow-md">
      <div className="flex items-center justify-between w-full max-w-6xl">
        <div className="flex items-center gap-2 text-gray-900 dark:text-white text-2xl font-bold" />

        <div className="flex gap-1 relative bg-gray-200 dark:bg-gray-800 rounded-full p-1 shadow-inner overflow-x-auto">
          {tabs.map((tab) => (
            <button
              key={tab.key}
              type="button"
              onClick={() => setSeccion(tab.key)}
              className={`relative flex items-center gap-2 px-5 py-2 rounded-full font-medium text-sm transition-all
                ${seccion === tab.key
                  ? "text-gray-900 dark:text-white"
                  : "text-gray-600 hover:text-gray-900 dark:text-gray-400 dark:hover:text-white"}`}
              aria-pressed={seccion === tab.key}
              aria-current={seccion === tab.key ? "page" : undefined}
              aria-label={tab.name}
              style={{ zIndex: 2 }}
            >
              <span className="text-lg">{tab.icon}</span>
              <span className="hidden sm:inline">{tab.name}</span>
              {seccion === tab.key && (
                <Motion.div
                  layoutId="pill-nav"
                  className="absolute inset-0 bg-purple-600/90 rounded-full"
                  style={{ zIndex: -1 }}
                  transition={{ type: "spring", stiffness: 500, damping: 30 }}
                />
              )}
            </button>
          ))}
        </div>

        <div className="flex items-center gap-2">
          <button
            type="button"
            onClick={toggle}
            className="flex items-center gap-2 text-sm font-semibold
                       text-gray-700 hover:text-gray-900 dark:text-gray-300 dark:hover:text-white transition"
            title="Cambiar tema"
            aria-label="Cambiar tema"
          >
            {isDark ? <FaSun /> : <FaMoon />}
            <span className="hidden sm:inline">{isDark ? "Claro" : "Oscuro"}</span>
          </button>

          <button
            type="button"
            onClick={onLogout}
            className="flex items-center gap-2 text-sm font-semibold text-gray-700 hover:text-red-500 dark:text-gray-300 dark:hover:text-red-400 transition"
            aria-label="Salir"
          >
            <FaSignOutAlt className="text-lg" />
            <span className="hidden sm:inline">Salir</span>
          </button>
        </div>
      </div>
    </nav>
  );
}

export default NavbarModern;