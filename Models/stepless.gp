// Параметры модели - наличие координатора, эксперимент с реагрегацией
bool CONTROL_CELL = true, SMASH = false;

// Типы клеток и сигнальных веществ
// За некоторыми исключениями клетка выделяет вещество, соответствующее своему типу
// Недифференциированные
int STEM = 0;
// Эктодерма
int ECTODERM = 1;
bool existsEctoderm = (sensorReaction[ECTODERM] > Simulation.ALMOST_ZERO);
// Эндодерма
int ENDODERM = 2;
bool existsEndoderm = (sensorReaction[ENDODERM] > Simulation.ALMOST_ZERO);
// Клетки рта (разделяющие эктодерму и эндодерму
int MOUTH = 3;
bool existsMouth = (sensorReaction[MOUTH] > Simulation.ALMOST_ZERO);

// Используется, чтобы пометить будущие клетки рта.
// В будущем избавимся, когда улучшим программу
int PREMOUTH = 33;
// Клетка-координатор.
int CONTROL = 4;
bool existsControl = (sensorReaction[CONTROL] > Simulation.ALMOST_ZERO);
// Вещество, сигнализирующее повреждение зародыша и необходимость вторично собраться в морулу.
int REAGGREGATE = 6;

bool maxLinksReached = (linkedCells.Count >= 10);

bool inReaggregate = (sensorReaction[REAGGREGATE] > 4.0);

// Проникающая способность сигнала эктодермы - падение на 1% на каждую единицу расстояния.
// Подобрана, чтобы при инвагинации эндодерма равномерно распределялась по эктодерме,
//  а не притягивалась в одну точку, слипаясь в комок
ProteinPenetration(ECTODERM, 0.99);
// Проникающая способность сигнала повреждения - падение на 0,1% на каждую единицу расстояния.
// Должен работать на большой дистанции, чтобы происходила реагрегация
ProteinPenetration(REAGGREGATE, 0.999);

// Случайный элемент к скорости движения клеток для устранения механистичности
movingSpeed = 0.1 + 0.2 * rnd;

// Клетки чуть больше - выглядят лучше
radius = 1.5;

// Клетки, кроме координатора, не имеют "состояния", сигнальное вещество, которое они выделают, определяется из их типа и положения.
if (cellType != CONTROL)
    DeSpillAll();

if (cellType == PREMOUTH)
    cellType = MOUTH;

// Внешнее событие - размазывание эмбриона по предметному стеклу
if (SMASH && step == 80)
{
    // Все клетки рассоединяются
    BreakFree();
    // Распределяются в пространстве в прямоугольной области 100x100x10
    position = position + new Vector(rnd * 100 - 50, rnd * 100 - 50, rnd * 20 - 10);
    // Тип клеток меняется на эктодерму (в будущем это нужно запрограммировать, как реакцию клеток на рассоединение)
    cellType = ECTODERM;
}

// Подсчёт числа клеток, находящихся рядом с текущей.
// Cчитаем, что они соприкасаются (чувствуют друг друга), даже если не связаны (см. ниже).
int nearCells = 0;
foreach (Cell cell in surroundingCells)
{
    if ((position - cell.position).mod < 5.0)
        nearCells++;
}

// Дальше для каждого типа клеток - своё поведение
// Эктодерма
if (cellType == ECTODERM)
{
    // Дифференциация в клетки рта при иммиграции
    if (P(0.01) && !existsControl && maxLinksReached && sensorReaction[ENDODERM] < 0.1 &&
        !inReaggregate)
    {
        cellType = MOUTH;
        Spill(MOUTH);
        return;
    }

    // Дифференциация в клетки эндодермы
    if (sensorReaction[CONTROL] > 0.025 ||
        (P(0.2) && !existsControl &&
            (sensorReaction[MOUTH] > 0.1 || sensorReaction[ENDODERM] > 0.1) &&
            (sensorReaction[ENDODERM] < 13)))
    {
        cellType = ENDODERM;
        Spill(ENDODERM);
        return;
    }

    // Если рядом менее 5 клеток - сигнализируем о реагрегации
    bool signalReaggregate = (nearCells < 5);
    Spill(signalReaggregate ? REAGGREGATE : ECTODERM);
    // Если происходит реагрегация - движемся по градиенту вещества эктодермы, чтобы слиться в морулу
    // Если нет - движемся против градиента вещества эктодермы, чтобы формировать и поддерживать полую бластулу.
    MoveGradient(ECTODERM, true, signalReaggregate || inReaggregate, 0.25);
}

// Эндодерма
if (cellType == ENDODERM)
{
    Vector outside = Vector.Invert(simulation.GetGradient(position, ECTODERM)).Normalize(1.0);
    bool coveredByEctoderm = false;
    Vector closest = null;
    // Определение "покрытости эктодермой"
    foreach (Cell cell in surroundingCells)
    {
        Vector difference = (cell.position - position).Normalize(1.0) - outside;
        if (cell.cellType == ECTODERM)
        {
            if (difference.Length < 1.0)
            {
                coveredByEctoderm = true;
                break;
            }
            else if (difference.Length < 2.0)
                closest = difference;
        }
    }
    // Если клетка эктодермы находится на расстоянии менее 1 в направлении "наружу" - движения не происходит
    // Если на расстоянии от 1 до 2 - движемся к этой клетке эктодермы
    // Иначе движемся по градиенту вещества, вырабатываемого эктодермой.
    if (!coveredByEctoderm)
    {
        if (closest != null)
            Move(closest, 0.3, true);
        else
            MoveGradient(ECTODERM, true, true, 0.05);
    }
    Spill(ENDODERM);
}

// Условие связывания клеток в упругий слой
if (!inReaggregate && P(0.1) && !maxLinksReached && existsEctoderm)
{
    foreach (Cell cell in surroundingCells)
    {
        // Связываются клетки одинакового типа и любые с координатором
        if (!linkedCells.Contains(cell) && (cellType == cell.cellType || cellType == CONTROL || cell.cellType == CONTROL))
        {
            // Будет привязана одна ближайшая клетка за раз
            LinkTo(cell);

            break;
        }
    }
}

// Клетки рта в данный момент не имеют особого поведения.
if (cellType == MOUTH) { }

// Разделение клеток на слои при иммиграции
// Клетки эктодермы и эндодермы должны отделиться и связываться вместе только через клетки рта
if ((existsMouth || existsControl) && cellType != MOUTH) {
    bool linkedEcto = false, linkedEndo = false, linkedMouth = false;

    // Определение типов связанных клеток
    foreach (Cell cell in linkedCells) {
        if (cell.cellType == ECTODERM)
            linkedEcto = true;
        if (cell.cellType == ENDODERM)
            linkedEndo = true;
        if (cell.cellType == MOUTH)
            linkedMouth = true;
    }

    if (linkedEcto && linkedEndo) {
        if (linkedMouth || existsControl)
        {
            // Образование клеток рта на границе слоёв
            cellType = PREMOUTH;
            return;
        } else {
            // Отсоединение клеток не совпавшего типа
            foreach (Cell cell in linkedCells.Copy())
            {
                if (cell.cellType != cellType)
                    UnlinkFrom(cell);
            }
        }
    }
}

// Клетка-координатор
if (cellType == CONTROL) {
    // Выбирается на поверхность зародыша
    if (EnvironmentalAccess < 0.15)
        MoveFromTheCrowd(false, 0.15);

    // Начинает вырабатывать сигнальное вещество, когда связана с достаточным числом клеток в бластуле, но иммиграция не началась.
    if (linkedCells.Count >= 7 && !existsEndoderm)
        Spill(CONTROL);
}

// Недифференциированная клетка
if (cellType == STEM) {
    // Дифференциация в эктодерму, когда зародыш достигает порогового размера, определяемого через концентрацию.
    if (sensorReaction[STEM] > 200 || existsEctoderm)
        cellType = ECTODERM;
    // Иначе деление с вероятностью 10%
    else if (P(0.1))
        SpawnWherever();

    Spill(STEM);
}

// Превращение "первой" клетки в координатор
if (CONTROL_CELL && step == 0) {
    SpawnWherever();
    cellType = CONTROL;
}
