using System.Text.Json;
using Application.Abstractions.Messaging;
using Domain.Dtos.Chat;
using Microsoft.KernelMemory;

namespace Infrastructure.Services.OpenAI;

public class FAQDataService : IFAQDataService
{
    private readonly IKernelMemory _memory;

    public FAQDataService(IKernelMemory memory)
    {
        _memory = memory;
    }

    public async Task InitializeFAQDataAsync()
    {
        var faqJsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "faqData.json");
        if (!File.Exists(faqJsonPath))
            throw new FileNotFoundException($"FAQ data file not found: {faqJsonPath}");

        var faqJson = await File.ReadAllTextAsync(faqJsonPath);
        var faqData = JsonSerializer.Deserialize<List<FAQItem>>(faqJson);
        if (faqData == null)
            throw new Exception("Failed to deserialize FAQ data from JSON.");

        for (var i = 0; i < faqData.Count; i++)
        {
            var faq = faqData[i];
            var documentId = $"faq-{i}";

            var alreadyIndexed = await _memory.IsDocumentReadyAsync(documentId);
            if (alreadyIndexed)
                continue;

            await _memory.ImportTextAsync(
                $"Q: {faq.Question}\nA: {faq.Answer}",
                documentId: documentId,
                tags: new TagCollection
                {
                    { "type", "faq" },
                    { "category", "task-management" }
                }
            );
        }
    }
}
