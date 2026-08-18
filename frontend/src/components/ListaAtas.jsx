export function ListaAtas({ atas, aoClicarWorkshop }) {
  return (
    <table className="lista-atas">
      <thead>
        <tr>
          <th>Workshop</th>
          <th>Data</th>
          <th>Descrição</th>
          <th>Colaboradores</th>
        </tr>
      </thead>
      <tbody>
        {atas.map((ata) => (
          <tr key={ata.id}>
            <td>
              <button className="link-workshop" onClick={() => aoClicarWorkshop(ata)}>
                {ata.workshop.nome}
              </button>
            </td>
            <td>{new Date(ata.workshop.dataRealizacao).toLocaleDateString('pt-BR')}</td>
            <td>{ata.workshop.descricao}</td>
            <td>{ata.totalColaboradores}</td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}
