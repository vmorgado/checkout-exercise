<template>
  <v-app id="inspire">
    <v-container class="fill-height" fluid>
      <v-row align="center" justify="center">
        <v-col cols="12" sm="8" md="4">
          <v-card class="elevation-12">
            <v-toolbar color="primary" dark flat>
              <v-toolbar-title>Payment Form</v-toolbar-title>
              <v-spacer></v-spacer>
            </v-toolbar>
            <v-card-text>
              <v-form>
                <v-text-field label="Full Name" name="name" prepend-icon="mdi-account" type="text"></v-text-field>
                <v-text-field label="Card Number" name="cardNumber" prepend-icon="mdi-card" type="text"></v-text-field>
                <v-row>
                  <v-col cols="4" sm="8" md="4">
                      
                     <v-text-field label="Expires (Year)" name="expireYear" type="text"></v-text-field>

                  </v-col>
                  <v-col cols="4" sm="8" md="4">
                     <v-text-field label="Expery (Month)" name="expireMonth" type="text"></v-text-field>
                  </v-col>
                  
                  <v-col cols="4" sm="8" md="4">
                      <v-text-field id="ccv" label="CCV" name="ccv" prepend-icon="mdi-lock" type="password"></v-text-field>

                  </v-col>
                </v-row>
                
              </v-form>
            </v-card-text>
            <v-card-actions>
              <v-spacer></v-spacer>
              <v-btn color="green">Pay</v-btn>
            </v-card-actions>
          </v-card>
        </v-col>
      </v-row>
    </v-container>
  </v-app>
</template>
<script>
export default {
  name: "PaymentForm",
  props: {
    
  },
  data: () => ({}),
};
</script>
