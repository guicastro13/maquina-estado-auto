public class AutomacaoContexto
{
    private readonly Dictionary<string, object> _dadosDinamicos = new Dictionary<string, object>();
    public string? JanelaMitra { get; set; }
    public void AdicionarDado(string chave, object valor)
    {
        _dadosDinamicos[chave] = valor;
    }
    public object? ObterDado(string chave)
    {
        if (_dadosDinamicos.TryGetValue(chave, out var valor))
        {
            return valor;
        }
        return null;
    }
}
public class CenarioFront
{
    private readonly List<Func<AutomacaoContexto, Task>> _acoes = new List<Func<AutomacaoContexto, Task>>();
    private int _ultimoIndiceExecutado = -1;
    private AutomacaoContexto _contexto;
    private string _name;
    public CenarioFront(IServiceProvider serviceProvider)
    {
        _name = "Validação Front";
        _contexto = new AutomacaoContexto();
    }
    protected void Executar()
    {
        string? janelaMitra = "Janela Aberta";
        if (janelaMitra is null)
            throw new Exception("Não foi possível realizar log in no Mitra.");
        _contexto.JanelaMitra = janelaMitra;
        StartCenario(_contexto);
    }
    private void StartCenario(AutomacaoContexto contexto)
    {
        Chamar(async ctx => await new Automacao().Func1(ctx))
            .Chamar(async ctx => await new Automacao().Func2(ctx))
            .Chamar(async ctx => await new Automacao().Func3(ctx))
            .Chamar(async ctx => await new Automacao().Func4(ctx))
            .Chamar(async ctx => await new Automacao().Func5(ctx));

        ExecutarAsync().Wait();
    }
    public async Task RetomarAsync()
    {
        Console.WriteLine($"Retomando a execução do passo {_ultimoIndiceExecutado + 2}");
        await ExecutarAsync();
    }
    public async Task ExecutarAsync()
    {
        for (int i = _ultimoIndiceExecutado + 1; i < _acoes.Count; i++)
        {
            try
            {
                Console.WriteLine($"Executando passo {i + 1}");
                await _acoes[i](_contexto);
                _ultimoIndiceExecutado = i;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na execução da função {i + 1}: {ex.Message}");
                break;
            }
        }
    }
    public void LimparSequencia()
    {
        _acoes.Clear();
        _ultimoIndiceExecutado = -1;
    }
    public CenarioFront Chamar(Func<AutomacaoContexto, Task> acao)
    {
        _acoes.Add(acao);
        return this;
    }
}
public class Automacao
{
    public async Task Func1(AutomacaoContexto contexto)
    {
        Console.WriteLine("Executando FuncAqui...");
        await Task.Delay(500);
        contexto.AdicionarDado("ResultadoFunc1", 42);
    }
    public async Task Func2(AutomacaoContexto contexto)
    {
        if (contexto.ObterDado("ResultadoFunc1") is int valor)
        {
            Console.WriteLine($"Executando Func2 com valor de parâmetro: {valor}");
        }
        else
        {
            Console.WriteLine("ResultadoFunc1 não encontrado.");
        }
        await Task.Delay(500);
    }
    public async Task Func3(AutomacaoContexto contexto)
    {
        Console.WriteLine("Executando Func3...");
        await Task.Delay(500);
        Random random = new Random();
        int numeroSorteado = random.Next(1, 11);
        if (numeroSorteado > 5)
        {
            throw new Exception("ERROR!!!");
        }
    }   
    public async Task Func4(AutomacaoContexto contexto)
    {
        Console.WriteLine("Pegando arquivo...");
        await Task.Delay(500);
    }
    public async Task Func5(AutomacaoContexto contexto)
    {
        Console.WriteLine("Fazendo isso...");
        await Task.Delay(500);
    }
}