CREATE OR REPLACE FUNCTION return_boolean(INOUT boolean_to_return BOOLEAN) RETURNS BOOLEAN
LANGUAGE sql
AS $$
  SELECT boolean_to_return;
$$;
