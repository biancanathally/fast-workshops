import { useState, useEffect } from 'react';

export function useDebounce(valor, delayMs = 300) {
  const [valorDebounced, setValorDebounced] = useState(valor);

  useEffect(() => {
    const timer = setTimeout(() => setValorDebounced(valor), delayMs);
    return () => clearTimeout(timer);
  }, [valor, delayMs]);

  return valorDebounced;
}
