using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TutorialSequence : MonoBehaviour
{
    [SerializeField] private float _gap = 0.4f;
    [SerializeField] private float _slowFade = 1.5f;

    private TutorialLog log;
    private TutorialSignals signals;
    private TutorialHighlight _targets;
    private TutorialTableHighlight _table;
    private BetTable _betTable;

    private readonly HashSet<TutorialSignal> _raised = new();
    private int _lastBetIndex = -1;

    [Inject]
    public void Construct(TutorialLog log, TutorialSignals signals,
        TutorialHighlight targets, TutorialTableHighlight table, BetTable betTable)
    {
        this.log = log;
        this.signals = signals;
        _targets = targets;
        _table = table;
        _betTable = betTable;

        this.signals.Raised += OnSignal;
        _betTable.ChangeAmountByIndex += OnBetChanged;
    }

    private void Start() => StartCoroutine(Run());

    private void OnDestroy()
    {
        if (signals != null) signals.Raised -= OnSignal;
        if (_betTable != null) _betTable.ChangeAmountByIndex -= OnBetChanged;
    }

    private void OnSignal(TutorialSignal s) => _raised.Add(s);

    private void OnBetChanged(int index, int amount)
    {
        if (amount <= 0) return;
        _lastBetIndex = index;
        signals.Raise(TutorialSignal.BetPlaced);
    }

    private IEnumerator Run()
    {
        Debug.Log("[Tutorial] Run started");
        yield return WaitSignal(TutorialSignal.TableEntered);

        // 1. Ставка
        _table.BlinkAllNumbers();
        log.ShowLog("Делайте ставку (ЛКМ)", 0f);
        yield return WaitSignal(TutorialSignal.BetPlaced);
        _table.Stop();

        // 2. Медленное исчезновение
        log.HideLog(_slowFade);
        yield return new WaitForSecondsRealtime(_slowFade + _gap);

        // 3. Число, цвет, дюжина
        _table.BlinkBet(_lastBetIndex);
        yield return Say("Делая ставку на число, вы одновременно делаете ставку на дюжину, цвет и число", 5f);
        _table.Stop();

        // 4. Множители
        _targets.Set(TutorialTargetId.Multipliers, true);
        yield return Say("У каждого из этих элементов свой множитель", 4f);
        _targets.Set(TutorialTargetId.Multipliers, false);

        // 5
        yield return Say("Вы можете отменить ставку (ПКМ)", 3.5f);

        // 6. Квота
        yield return SayTarget("Ваша задача – побить квоту", 3.5f, TutorialTargetId.Quota);

        // 7. Рулетка (висит, пока игрок не запустит кручение)
        _targets.Set(TutorialTargetId.Roulette, true);
        log.ShowLog("Крутите рулетку", 0f);
        yield return WaitSignal(TutorialSignal.SpinStarted);
        _targets.Set(TutorialTargetId.Roulette, false);
        yield return HideFast();

        // 8. Кручение
        yield return WaitSignal(TutorialSignal.SpinFinished);

        // 9-10
        yield return SayTarget("Вы можете завершить ставку, нажав на шарик или крутить заново", 5f, TutorialTargetId.Ball);
        yield return Say("Первый реролл бесплатный, после этого вы будете терять 25% выигрыша за каждое новое кручение", 6f);

        // 11. Квота выполнена
        yield return WaitSignal(TutorialSignal.QuotaCompleted);
        _targets.Set(TutorialTargetId.FinishBetButton, true, blink: true);
        log.ShowLog("Вы выиграли, можете завершить ставку, нажав соответствующую кнопку", 0f);
        yield return WaitSignal(TutorialSignal.BetFinished);
        _targets.Set(TutorialTargetId.FinishBetButton, false);
        yield return HideFast();

        // 12. Отошли от стола (висит до входа в магазин)
        yield return WaitSignal(TutorialSignal.LeftTable);
        log.ShowLog("Найдите магазин, там можно купить необходимые улучшения", 0f);
        yield return WaitSignal(TutorialSignal.ShopEntered);

        // 13-19. Магазин
        yield return Say("За заработанные золотые фишки можно купить особые фишки, увеличение количества серебра для ставки, еду, и шарики", 6f);
        yield return Say("У каждой категории есть свои ограничения", 3.5f);
        yield return SayTarget("Под фишки есть только 5 мест, выбирайте с умом", 4f, TutorialTargetId.ShopChips);
        yield return SayTarget("Еда обновляется в магазине после каждой третьей квоты", 4f, TutorialTargetId.ShopFood);
        yield return SayTarget("Цена на покупку серебра растет с каждой покупкой", 4f, TutorialTargetId.ShopConverter);
        yield return SayTarget("Всего доступно два шарика каждые 3 квоты", 4f, TutorialTargetId.ShopBallMachine);
        yield return Say("После покупок вернитесь к столу", 3.5f);

        // 20. Третья квота
        yield return WaitSignal(TutorialSignal.ThirdQuotaStarted);
        yield return Say("После каждой третьей квоты вы получите дополнительную награду", 4f);

        // 21. Босс-фишка (висит, пока игрок не придет к алтарю)
        yield return WaitSignal(TutorialSignal.ThirdQuotaRewardsChosen);
        log.ShowLog("Вы получили босс-фишку, найдите алтарь плаков, чтобы ее потратить", 0f);
        yield return WaitSignal(TutorialSignal.AltarEntered);

        // 22
        yield return Say("Плак – улучшение, которое меняет поле, выбирайте с умом", 5f);
    }

    // ---- Хелперы ----

    private IEnumerator Say(string msg, float duration)
    {
        log.ShowLog(msg, duration);
        yield return new WaitForSecondsRealtime(
            TutorialLog.FadeInDuration + duration + TutorialLog.FadeOutDuration + _gap);
    }

    private IEnumerator SayTarget(string msg, float duration, TutorialTargetId id)
    {
        _targets.Set(id, true);
        yield return Say(msg, duration);
        _targets.Set(id, false);
    }

    private IEnumerator HideFast()
    {
        log.HideLog();
        yield return new WaitForSecondsRealtime(TutorialLog.FadeOutDuration + _gap);
    }

    private IEnumerator WaitSignal(TutorialSignal signal)
    {
        while (!_raised.Remove(signal))
            yield return null;
    }
}