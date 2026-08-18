export function FiltrosBar({ filtros, aoMudar }) {
  return (
    <div className="filters">
      <div className="field">
        <label htmlFor="f-colab">Colaborador</label>
        <input
          id="f-colab"
          type="text"
          value={filtros.colaboradorNome}
          onChange={(e) => aoMudar({ ...filtros, colaboradorNome: e.target.value })}
          placeholder="Buscar por colaborador"
        />
      </div>

      <div className="field">
        <label htmlFor="f-work">Workshop</label>
        <input
          id="f-work"
          type="text"
          value={filtros.workshopNome}
          onChange={(e) => aoMudar({ ...filtros, workshopNome: e.target.value })}
          placeholder="Buscar por workshop"
        />
      </div>

      <div className="field">
        <label htmlFor="f-date">Data de realização</label>
        <input
          id="f-date"
          type="date"
          value={filtros.data}
          onChange={(e) => aoMudar({ ...filtros, data: e.target.value })}
        />
      </div>
    </div>
  );
}
