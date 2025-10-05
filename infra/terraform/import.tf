# Import dos recursos existentes
# Execute estes comandos se os recursos já existirem:

# terraform import aws_secretsmanager_secret.connection_string fastfood/db/connection-string
# terraform import aws_secretsmanager_secret.jwt_settings fastfood/jwt/settings
# terraform import aws_secretsmanager_secret_version.connection_string_value fastfood/db/connection-string:AWSCURRENT
# terraform import aws_secretsmanager_secret_version.jwt_settings_value fastfood/jwt/settings:AWSCURRENT

# Import da função Lambda existente
# terraform import aws_lambda_function.auth FastFoodAutenticacao
