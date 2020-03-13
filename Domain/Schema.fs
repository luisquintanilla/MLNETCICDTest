namespace Domain

module Schema =

    open Microsoft.ML.Data

    [<CLIMutable>]
    type ModelInput = {
        [<LoadColumn(0)>] Comment: string
        [<LoadColumn(1); ColumnName("Label")>] Sentiment: bool
    }

    [<CLIMutable>]
    type ModelOutput = {
        [<ColumnName("PredictedLabel")>] PredictedSentiment: string
        Score: float32 array
    }

