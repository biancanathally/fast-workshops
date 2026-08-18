export function FiltrosBar({ filtros, aoMudar }) {
  return (
    <div className="filtros-bar">
      <label>
        Colaborador
        <input
          type="text"
          value={filtros.colaboradorNome}
          onChange={(e) => aoMudar({ ...filtros, colaboradorNome: e.target.value })}
          placeholder="Buscar por colaborador"
        />
      </label>

      <label>
        Workshop
        <input
          type="text"
          value={filtros.workshopNome}
          onChange={(e) => aoMudar({ ...filtros, workshopNome: e.target.value })}
          placeholder="Buscar por workshop"
        />
      </label>

      <label>
        Data de realização
        <input
          type="date"
          value={filtros.data}
          onChange={(e) => aoMudar({ ...filtros, data: e.target.value })}
        />
      </label>
    </div>
  );
}
