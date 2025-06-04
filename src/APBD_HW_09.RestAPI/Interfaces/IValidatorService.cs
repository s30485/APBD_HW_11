namespace APBD_HW_09.RestAPI.Interfaces;

public interface IValidatorService
{
    bool Validate<T>(T model, out Dictionary<string, string[]> outErrors);
}