CREATE OR REPLACE FUNCTION output_nullable(
    nullable_in INT,
    OUT nullable_out INT
) AS $$
BEGIN
    nullable_out = nullable_in;
END
$$ LANGUAGE plpgsql;
