/*
    Classe: 

    [Não permite mais de uma por objeto]
    StateMachine - Abstrata. As máquinas de estados devem derivar desta classe.

    Variáveis privadas: 

    - string status: Apenas para debug. Informa o estado atual da State Machine no inspector.

    - List<State> statesList: Armazena uma referência aos componentes State adicionados ao gameObject. Assim na hora de definir novos estados o processo é mais curto.

    
    Propriedades: 

    - CurrentState: retorna o estado atual ativo. (Apenas Leitura).


    Métodos: 

    - SetState(State state): Define o estado (parâmetro) para a máquina de estados. 
      Retorna verdadeiro se o estado foi definido.

    - SetState<StateType>() where StateType: State : Força a máquina de estado a entrar no estado (StateType), mesmo que esta não o possua. Gera um NullReferenceException na primeira tentativa caso o estado não seja encontrado em nenhum lugar do objeto. Porém ao final será adicionado.
      Retorna verdadeiro se o estado foi definido.

    - SetAndGetState<StateType>() where StateType: State : O mesmo de SetState. 
      Retorna o estado alvo caso definido. Se não for definido retorna null.

    - GetState<StateType>() where StateType: State : Procura pelo estado (StateType) na máquina de estados. 
      Retorna o estado alvo, se não encontrado retorna null.

========================================================================================================================================================

    Classe: 

    [Requer uma máquina de estado no gameObject]
    State - Abstrata. Os estados devem derivar desta classe.

    Variáveis privadas: 

    - StateMachine machine: A máquina de estado ao qual este pertence. É defnida automaticamente.

    
    Propriedades: 

    Nenhuma. Pode ser adicionado conforme necessidade.


    Métodos: 

    public void Initialize(StateMachine machine): Método público para ser chamado de forma externa. Inicializa o estado na máquina de estado (parâmetro). Uso recomendado: Definir valriáveis ou valores logo que o estado for definido na máquina de estado. Não precisa ser chamado sempre.
    
    public void StateEnter(): Método público para ser chamado de forma externa. Função que executa a entrada do estado e lida com a ativação/desativação.

    public void StateExit(): O mesmo que StateEnter() porém executa funções de saída do estado.

    --- Métodos internos de cada estado ----

    protected virtual void OnStateInitialize(StateMachine machine = null): Contém toda a lógica de inicialização do estado. Pode ser editado em cada estado. Não necessariamente precisa ser implementada. É chamada automaticamente no momento que executa Initialize(). Por padrão define a própria machine(variável) por machine(parâmetro). Recomendado sempre usar base.Initialize()

    protected virtual void OnStateEnter(): Contém a lógica de entrada do estado. Pode ser editado em cada estado. Não necessariamente precisa ser implementada e por padrão não possui nenhuma lógica.

    protected virtual void ProcessState(): Contém a lógica de processamento do estado. Este por sua vez precisa ser chamado em um game loop(Update/FixedUpdate/LateUpdate) ou Coroutine persistente de acordo com cada estado, mas isso depende do estado em sí. Não necessariamente precisa ser implementada. Não é chamada em momento nenhum e por padrão não possui nenhuma lógica.

    protected virtual void OnStateExit(): Contém a lógica de saída do estado. Pode ser editado em cada estado. Não necessariamente precisa ser implementada e por padrão não possui nenhuma lógica.

*/