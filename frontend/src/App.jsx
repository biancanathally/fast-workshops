import { useState, useEffect } from 'react';
import { FiltrosBar } from './components/FiltrosBar';
import { ListaAtas } from './components/ListaAtas';
import { DetalheWorkshop } from './components/DetalheWorkshop';
import { EstadoVazio } from './components/EstadoVazio';
import { GraficoBarras } from './components/GraficoBarras';
import { GraficoPizza } from './components/GraficoPizza';
import { useDebounce } from './hooks/useDebounce';
import { listarAtas, listarColaboradores, ApiError } from './services/api';
import './App.css';

export default function App() {
  const [filtros, setFiltros] = useState({ colaboradorNome: '', workshopNome: '', data: '' });
  const [atas, setAtas] = useState([]);
  const [estado, setEstado] = useState('loading');
  const [mensagemErro, setMensagemErro] = useState('');
  const [ataSelecionada, setAtaSelecionada] = useState(null);

  const [colaboradoresTodos, setColaboradoresTodos] = useState([]);
  const [atasTodas, setAtasTodas] = useState([]);
  const [graficosCarregando, setGraficosCarregando] = useState(true);

  const filtrosDebounced = useDebounce(filtros, 300);

  useEffect(() => {
    const controller = new AbortController();
    async function buscar() {
      setEstado('loading');
      try {
        const resultado = await listarAtas(filtrosDebounced, controller.signal);
        setAtas(resultado);
        setEstado(resultado.length === 0 ? 'vazio' : 'ok');
      } catch (err) {
        if (err.name === 'AbortError') return;
        setMensagemErro(err instanceof ApiError ? err.message : 'Erro ao buscar atas.');
        setEstado('erro');
      }
    }
    buscar();
    return () => controller.abort();
  }, [filtrosDebounced]);

  useEffect(() => {
    const controller = new AbortController();
    async function buscarDadosGraficos() {
      setGraficosCarregando(true);
      try {
        const [colaboradores, todasAtas] = await Promise.all([
          listarColaboradores(controller.signal),
          listarAtas({}, controller.signal),
        ]);
        setColaboradoresTodos(colaboradores);
        setAtasTodas(todasAtas);
      } catch (err) {
        if (err.name === 'AbortError') return;
        console.error('Erro ao carregar dados dos gráficos:', err);
      } finally {
        setGraficosCarregando(false);
      }
    }
    buscarDadosGraficos();
    return () => controller.abort();
  }, []);

  return (
    <div className="page">
      <header className="masthead">
        <div>
          <p className="eyebrow">registro de presença</p>
          <h1>FAST Workshops</h1>
          <p className="sub">Acompanhamento de atas e presença em workshops internos.</p>
        </div>
        <div className="stamps">
          <div className="stamp">
            <span className="num">{atasTodas.length}</span>
            <span className="lbl">workshops</span>
          </div>
          <div className="stamp">
            <span className="num">{colaboradoresTodos.length}</span>
            <span className="lbl">colaboradores</span>
          </div>
        </div>
      </header>

      <FiltrosBar filtros={filtros} aoMudar={setFiltros} />

      <div className="board">
        <div>
          <p className="section-label">Atas</p>

          {estado === 'ok'
            ? <ListaAtas atas={atas} aoClicarWorkshop={setAtaSelecionada} />
            : <EstadoVazio tipo={estado} mensagem={mensagemErro} />}
        </div>

        <div>
          <p className="section-label">Indicadores</p>

          {graficosCarregando ? (
            <p className="state-note">Carregando indicadores…</p>
          ) : (
            <>
              <GraficoBarras colaboradores={colaboradoresTodos} />
              <GraficoPizza atas={atasTodas} />
            </>
          )}

          <div className="hint">
            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <circle cx="12" cy="12" r="10" />
              <path d="M12 16v-4M12 8h.01" />
            </svg>
            clique no nome do workshop na lista para ver o detalhe
          </div>
        </div>
      </div>

      <DetalheWorkshop ata={ataSelecionada} aoFechar={() => setAtaSelecionada(null)} />

      <footer className="legend-note">FAST Soluções — desafio técnico fullstack</footer>
    </div>
  );
}
