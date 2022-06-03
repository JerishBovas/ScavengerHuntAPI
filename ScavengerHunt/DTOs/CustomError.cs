using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ScavengerHunt.DTOs;

public class CustomError
{
    public string Title { get; set; }
    public int Status { get; set; }
    public List<string> Errors { get; set; }
    public CustomError(ActionContext context)
    {
        Title = "Internal server error";
        Status = 500;
        Errors = ConstructErrorMessages(context);
    }

    public CustomError(string title, int status, string[] errs)
    {
        Title = title;
        Status = status;
        Errors = new();
        foreach (var err in errs)
        {
            Errors.Add(err);
        }
    }

    private List<string> ConstructErrorMessages(ActionContext context)
    {
        List<string> err = new();
        foreach (var keyModelStatePair in context.ModelState)
        {
            var errors = keyModelStatePair.Value.Errors;
            if (errors != null && errors.Count > 0)
            {
                for (var i = 0; i < errors.Count; i++)
                {
                    err.Add(GetErrorMessage(errors[i]));
                }
            }
        }
        return err;
    }

    string GetErrorMessage(ModelError error)
    {
        return string.IsNullOrEmpty(error.ErrorMessage) ?
            "The input was not valid." :
            error.ErrorMessage;
    }
}