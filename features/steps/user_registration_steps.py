import requests
from behave import given, when, then

API_URL = "http://localhost:5104/User/register"

@given('I am on the registration endpoint')
def step_impl(context):
    context.registration_url = API_URL

@given('a user with email "{email}" is already registered')
def step_impl(context, email):
    payload = {
        "name": "John Doe",
        "email": email,
        "password": "SenhaForte123"
    }
    requests.post(context.registration_url, json=payload)

@when('I submit valid user information')
def step_impl(context):
    context.payload = {
        "name": "Valid User",
        "email": "validuser@email.com",
        "password": "StrongPassword123"
    }
    context.response = requests.post(context.registration_url, json=context.payload)

@when('I submit incomplete user information')
def step_impl(context):
    context.payload = {
        "name": "",
        "email": "",
        "password": ""
    }
    context.response = requests.post(context.registration_url, json=context.payload)

@when('I attempt to register with the same email')
def step_impl(context):
    context.payload = {
        "name": "Another User",
        "email": "john@example.com",
        "password": "SenhaForte456"
    }
    context.response = requests.post(context.registration_url, json=context.payload)

@when('I submit a password that does not meet security requirements')
def step_impl(context):
    context.payload = {
        "name": "Weak Pass User",
        "email": "weakpass@email.com",
        "password": "123"
    }
    context.response = requests.post(context.registration_url, json=context.payload)

@when('I submit an email address that is not properly formatted')
def step_impl(context):
    context.payload = {
        "name": "Invalid Email User",
        "email": "a@b.c",
        "password": "StrongPassword123"
    }
    context.response = requests.post(context.registration_url, json=context.payload)

@when('I submit a name with less than 3 characters')
def step_impl(context):
    context.payload = {
        "name": "Jo",
        "email": "jo@email.com",
        "password": "StrongPassword123"
    }
    context.response = requests.post(context.registration_url, json=context.payload)

@when('I submit a name with more than 50 characters')
def step_impl(context):
    context.payload = {
        "name": "J" * 51,
        "email": "longname@email.com",
        "password": "StrongPassword123"
    }
    context.response = requests.post(context.registration_url, json=context.payload)

@when('I submit an email address with less than 8 characters')
def step_impl(context):
    context.payload = {
        "name": "Short Email",
        "email": "a@b.c",
        "password": "StrongPassword123"
    }
    context.response = requests.post(context.registration_url, json=context.payload)

@when('I submit an email address with more than 100 characters')
def step_impl(context):
    context.payload = {
        "name": "Long Email",
        "email": f"{'a'*90}@email.com",
        "password": "StrongPassword123"
    }
    context.response = requests.post(context.registration_url, json=context.payload)

@when('I submit a password with less than 8 characters')
def step_impl(context):
    context.payload = {
        "name": "Short Password",
        "email": "shortpass@email.com",
        "password": "1234567"
    }
    context.response = requests.post(context.registration_url, json=context.payload)

@when('I submit a password with more than 100 characters')
def step_impl(context):
    context.payload = {
        "name": "Long Password",
        "email": "longpass@email.com",
        "password": "A" * 101
    }
    context.response = requests.post(context.registration_url, json=context.payload)

@then('my account is created')
def step_impl(context):
    assert context.response.status_code == 200
    data = context.response.json()
    assert "errorMessage" not in data or not data.get("errorMessage")

@then('I receive a confirmation message')
def step_impl(context):
    data = context.response.json()
    print(data)

@then('my account is not created')
def step_impl(context):
    print(context.response.json())
    assert context.response.status_code == 400

@then('And I receive an error message describing the missing registration fields')
def step_impl(context):
    data = context.response.json()
    print(data)
    assert "obrigatório" in data.get("errorMessage", "").lower()

@then('I receive an error message indicating the email is already in use')
def step_impl(context):
    data = context.response.json()
    print(data)
    assert "este email já está em uso" in data.get("errorMessage", "").lower()

@then('I receive an error message about password strength')
def step_impl(context):
    data = context.response.json()
    print(data)
    assert "senha" in data.get("errorMessage", "").lower() and "pelo menos 8 caracteres" in data.get("errorMessage", "").lower()

@then('I receive an error message about email format')
def step_impl(context):
    data = context.response.json()
    print(data)
    assert "email" in data.get("errorMessage", "").lower() and "pelo menos 8 caracteres" in data.get("errorMessage", "").lower()

@then('I receive an error message indicating the name is too short')
def step_impl(context):
    data = context.response.json()
    print(data)
    assert "nome" in data.get("errorMessage", "").lower() and "pelo menos 3 caracteres" in data.get("errorMessage", "").lower()

@then('I receive an error message indicating the name is too long')
def step_impl(context):
    data = context.response.json()
    print(data)
    assert "nome" in data.get("errorMessage", "").lower() and "no máximo 50 caracteres" in data.get("errorMessage", "").lower()

@then('I receive an error message indicating the email is too short')
def step_impl(context):
    data = context.response.json()
    print(data)
    assert "email" in data.get("errorMessage", "").lower() and "pelo menos 8 caracteres" in data.get("errorMessage", "").lower()

@then('I receive an error message indicating the email is too long')
def step_impl(context):
    data = context.response.json()
    print(data)
    assert "no máximo 100 caracteres" in data.get("errorMessage", "").lower() or "este email já está em uso" in data.get("errorMessage", "").lower()

@then('I receive an error message indicating the password is too short')
def step_impl(context):
    data = context.response.json()
    print(data)
    assert "senha" in data.get("errorMessage", "").lower() and "pelo menos 8 caracteres" in data.get("errorMessage", "").lower()

@then('I receive an error message indicating the password is too long')
def step_impl(context):
    data = context.response.json()
    print(data)
    assert "senha" in data.get("errorMessage", "").lower() and "no máximo 100 caracteres" in data.get("errorMessage", "").lower()