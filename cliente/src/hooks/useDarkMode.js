import { useEffect, useState } from "react";

export default function useDarkMode() {
  const [isDark, setIsDark] = useState(false);

  useEffect(() => {
    const pref = localStorage.getItem("modoOscuro") === "true";
    if (pref) document.documentElement.classList.add("dark");
    setIsDark(pref);
  }, []);

  const toggle = () => {
    document.documentElement.classList.toggle("dark");
    const now = document.documentElement.classList.contains("dark");
    localStorage.setItem("modoOscuro", now ? "true" : "false");
    setIsDark(now);
  };

  return { isDark, toggle };
}
