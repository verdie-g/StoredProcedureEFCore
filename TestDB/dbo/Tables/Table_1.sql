CREATE TABLE [dbo].[Table_1] (
    [id]     BIGINT        IDENTITY (1, 1) NOT NULL,
    [name]   VARCHAR (255) NOT NULL,
    [date]   DATETIME      NOT NULL,
    [active] BIT           NOT NULL,
    CONSTRAINT [PK_Table_1] PRIMARY KEY CLUSTERED ([id] ASC)
);

