namespace APBD_HW_11.RestAPI.Interfaces;

public interface IValidatorService
{
    bool Validate<T>(T model, out Dictionary<string, string[]> outErrors);
}