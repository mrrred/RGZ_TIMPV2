namespace RGZ_TIMP.Services;

public interface IDialogService
{
    string? ShowNodeCodeDialog(string currentCode);
    (int predicate, int delay)? ShowEdgeDialog(int predicate, int delay);
    int? ShowAnimationSettingsDialog(int currentDurationSec);
    string? ShowSaveFileDialog(string defaultName);
    string? ShowOpenFileDialog();
    void ShowHelpDialog();
}