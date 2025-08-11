import requests
from behave import given, when, then

API_URL = "http://localhost:5104/Auth/login"
REGISTER_API_URL = "http://localhost:5104/User/register"

@given('I am a registered user with email "{email}" and a valid password')
def step_impl(context, email):
    context.email = email
    context.password = "SenhaValida123"
    payload = {
        "name": "John Doe",
        "email": email,
        "password": context.password
    }
    requests.post(REGISTER_API_URL, json=payload)

@given('I am a registered user with email "{email}"')
def step_impl(context, email):
    context.email = email
    context.password = "SenhaValida123"
    payload = {
        "name": "John Doe",
        "email": email,
        "password": context.password
    }
    requests.post(REGISTER_API_URL, json=payload)

@given('no user is registered with email "{email}"')
def step_impl(context, email):
    context.email = email
    context.password = "SenhaValida123"
    # Não registra este usuário

@given('I am on the login endpoint')
def step_impl(context):
    context.login_url = API_URL

@when('I submit my email and password to the login endpoint')
def step_impl(context):
    payload = {
        "email": context.email,
        "password": context.password
    }
    context.response = requests.post(API_URL, json=payload)

@when('I submit my email and an incorrect password to the login endpoint')
def step_impl(context):
    payload = {
        "email": context.email,
        "password": "SenhaIncorreta456"
    }
    context.response = requests.post(API_URL, json=payload)

@when('I attempt to log in with "{email}" and any password')
def step_impl(context, email):
    payload = {
        "email": email,
        "password": "QualquerSenha123"
    }
    context.response = requests.post(API_URL, json=payload)

@when('I submit a request missing the email or password')
def step_impl(context):
    payload = {
        "email": "",
        "password": ""
    }
    context.response = requests.post(API_URL, json=payload)

@then('I receive a valid JWT token')
def step_impl(context):
    print("Status code:", context.response.status_code)
    print("Raw response:", context.response.text)
    assert context.response.status_code == 200
    assert context.response.text
    data = context.response.json()
    assert "jwtToken" in data and data["jwtToken"]
    assert isinstance(data["jwtToken"], str) and len(data["jwtToken"]) > 20

@then('I am authenticated')
def step_impl(context):
    print("Status code:", context.response.status_code)
    print("Raw response:", context.response.text)
    assert context.response.status_code == 200
    assert context.response.text
    data = context.response.json()
    assert data.get("isAuthenticated") is True

@then('I do not receive a JWT token')
def step_impl(context):
    print("Status code:", context.response.status_code)
    print("Raw response:", context.response.text)
    # Aceita 400 ou 401 como erro de autenticação
    assert context.response.status_code in [400, 401]
    if context.response.text:
        data = context.response.json()
        assert "jwtToken" not in data or not data["jwtToken"]

@then('I receive an error message indicating invalid credentials')
def step_impl(context):
    print("Status code:", context.response.status_code)
    print("Raw response:", context.response.text)
    assert context.response.status_code in [400, 401]
    if context.response.text:
        data = context.response.json()
        assert "credenciais inválidas" in data.get("errorMessage", "").lower()

@then('I receive an error message describing the missing login fields')
def step_impl(context):
    print("Status code:", context.response.status_code)
    print("Raw response:", context.response.text)
    assert context.response.status_code in [400, 401]
    if context.response.text:
        data = context.response.json()
        msg = data.get("errorMessage", "").lower()
        assert "must not be empty" in msg